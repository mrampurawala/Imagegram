using Imagegram.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Infrastructure.Filters
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomIncludeJsonSerializerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objResult)
            {
                if (context != null && objResult.Value != null)
                {
                    string fieldsToInclude = context.HttpContext.Request.Query["fields"];

                    objResult.Formatters.Add(new JsonOutputFormatter(
                    new JsonSerializerSettings
                    {
                        ContractResolver = new DynamicContractResolver(objResult.Value.GetType(), fieldsToInclude)
                    },
                    context.HttpContext.RequestServices.GetRequiredService<ArrayPool<char>>()));
                }
            }
            base.OnActionExecuted(context);
        }
    }

    internal class DynamicContractResolver : DefaultContractResolver
    {
        private readonly Type _targetType;
        private readonly string _fieldsToInclude;
        private readonly List<string> _targetFields;
        private readonly List<PropertyFilter> _childSerializedObjects;

        public DynamicContractResolver(Type targetType, string fieldsToInclude)
        {
            _targetType = targetType;
            _fieldsToInclude = fieldsToInclude;
            _targetFields = new List<string>();
            _childSerializedObjects = new List<PropertyFilter>();
            NamingStrategy = new CamelCaseNamingStrategy { ProcessDictionaryKeys = true, OverrideSpecifiedNames = true };
            PopulateIncludeFields(_fieldsToInclude);
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> serializedProps = new List<JsonProperty>();
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            IList<JsonProperty> childFilterProps = properties.Where(e => e.AttributeProvider.GetAttributes(typeof(ChildObjectFilterAttribute), true).Count > 0).ToList();
            if (IsSameType(type, _targetType))
            {
                if (_fieldsToInclude == null)
                {
                    serializedProps.AddRange(properties);
                }
                else
                {
                    foreach (JsonProperty property in properties)
                    {
                        if ((property.Required == Required.Always) ||
                            (_targetFields.Exists(e => e.Equals(property.PropertyName, StringComparison.OrdinalIgnoreCase))))
                        {
                            serializedProps.Add(property);
                            if ((property.PropertyType != typeof(string)) && ((property.PropertyType.IsClass) || (property.PropertyType.IsGenericType && property.PropertyType.GetGenericArguments().FirstOrDefault() != typeof(string) && property.PropertyType.GetGenericArguments().First().IsClass)))
                            {
                                Type propertyType = property.PropertyType.IsClass ? property.PropertyType : property.PropertyType.GetGenericArguments().First();
                                JsonProperty cofp = childFilterProps.Find(e => e.PropertyName.Equals(property.PropertyName, StringComparison.OrdinalIgnoreCase));
                                if (cofp == null)
                                {
                                    _childSerializedObjects.Add(new PropertyFilter(property.PropertyName, propertyType, false));
                                }
                                else
                                {
                                    ChildObjectFilterAttribute cof = (ChildObjectFilterAttribute)cofp.AttributeProvider.GetAttributes(typeof(ChildObjectFilterAttribute), true).FirstOrDefault();
                                    _childSerializedObjects.Add(new PropertyFilter(property.PropertyName, propertyType, cof.CanFilter));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if ((_targetFields == null) || (_targetFields.Count == 0) || (_childSerializedObjects.Count == 0))
                {
                    serializedProps.AddRange(properties);
                }
                else
                {
                    PropertyFilter pf = _childSerializedObjects.FirstOrDefault();
                    if (pf != null && pf.PropertyType == type && pf.CanFilter)
                    {
                        _childSerializedObjects.RemoveAt(0);
                        List<PropertyFilter> tempChildSerializedObjects = new List<PropertyFilter>();
                        foreach (JsonProperty property in properties)
                        {
                            if ((property.Required == Required.Always) ||
                                (_targetFields.Exists(e => e.Equals(property.PropertyName, StringComparison.OrdinalIgnoreCase))))
                            {
                                serializedProps.Add(property);
                                if ((property.PropertyType != typeof(string)) && ((property.PropertyType.IsClass) || (property.PropertyType.IsGenericType && property.PropertyType.GetGenericArguments().FirstOrDefault() != typeof(string) && property.PropertyType.GetGenericArguments().First().IsClass)))
                                {
                                    Type propertyType = property.PropertyType.IsClass ? property.PropertyType : property.PropertyType.GetGenericArguments().First();
                                    JsonProperty cofp = childFilterProps.Find(e => e.PropertyName.Equals(property.PropertyName, StringComparison.OrdinalIgnoreCase));
                                    if (cofp == null)
                                    {
                                        tempChildSerializedObjects.Add(new PropertyFilter(property.PropertyName, propertyType, false));
                                    }
                                    else
                                    {
                                        ChildObjectFilterAttribute cof = (ChildObjectFilterAttribute)cofp.AttributeProvider.GetAttributes(typeof(ChildObjectFilterAttribute), true).FirstOrDefault();
                                        tempChildSerializedObjects.Add(new PropertyFilter(property.PropertyName, propertyType, cof.CanFilter));
                                    }
                                }
                            }
                        }
                        if (tempChildSerializedObjects.Count > 0)
                            _childSerializedObjects.InsertRange(0, tempChildSerializedObjects);

                        if (serializedProps.Count == 0) serializedProps.AddRange(properties);
                    }
                    else
                    {
                        serializedProps.AddRange(properties);
                    }
                }
            }
            return serializedProps;
        }

        private void PopulateIncludeFields(string fieldsToInclude)
        {
            if (!string.IsNullOrWhiteSpace(fieldsToInclude))
            {
                string[] arrFields = Array.ConvertAll(fieldsToInclude.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries), s => s.Trim());
                foreach (string field in arrFields)
                {
                    GetIncludeProperties(field, _targetType, new List<string>(), MemberSerialization.OptOut);
                }
            }
        }

        private bool GetIncludeProperties(string targetField, Type classType, List<string> parents, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(classType, memberSerialization);
            if (properties.Exists(e => e.PropertyName.Equals(targetField, StringComparison.OrdinalIgnoreCase)))
            {
                parents.Add(targetField);
                AddTargetFields(parents);
                return true;
            }
            else
            {
                foreach (JsonProperty property in properties.Where(e => (e.PropertyType != typeof(string)) && ((e.PropertyType.IsClass) || (e.PropertyType.IsGenericType && e.PropertyType.GetGenericArguments().FirstOrDefault() != typeof(string) && e.PropertyType.GetGenericArguments().First().IsClass))))
                {
                    Type propertyType = property.PropertyType.IsClass ? property.PropertyType : property.PropertyType.GetGenericArguments().First();
                    List<string> newParents = new List<string>(parents);
                    newParents.Add(property.PropertyName);
                    if (GetIncludeProperties(targetField, propertyType, newParents, memberSerialization)) break;
                }
                return false;
            }
        }

        private void AddTargetFields(List<string> targetFields)
        {
            if (targetFields != null && targetFields.Count > 0)
            {
                foreach (string tf in targetFields)
                {
                    if (!_targetFields.Exists(e => e.Equals(tf, StringComparison.OrdinalIgnoreCase)))
                        _targetFields.Add(tf);
                }
            }
        }

        private bool IsSameType(Type a, Type b)
        {
            if (a == b) return true;
            if (a.IsSubclassOf(b) || b.IsSubclassOf(a)) return true;
            return false;
        }
    }

    internal class PropertyFilter
    {
        public string PropertyName { get; }
        public Type PropertyType { get; }
        public bool CanFilter { get; }

        public PropertyFilter(string propertyName, Type propertyType, bool canFilter)
        {
            this.PropertyName = propertyName;
            this.PropertyType = propertyType;
            this.CanFilter = canFilter;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ChildObjectFilterAttribute : Attribute
    {
        private bool _canFilter;

        public ChildObjectFilterAttribute()
        {
            this._canFilter = true;
        }

        public ChildObjectFilterAttribute(bool canFilter)
        {
            this._canFilter = canFilter;
        }

        public virtual bool CanFilter
        {
            get { return this._canFilter; }
        }
    }
}
