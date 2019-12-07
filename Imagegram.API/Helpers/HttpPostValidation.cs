using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Imagegram.API.Helpers
{
    public class HTTPPostValidation : ContextValidation
    {
        public HTTPPostValidation(ActionExecutingContext context) : base(context)
        {
        }
        public override List<string> getErrorList()
        {
            if (_context.HttpContext.Request.ContentType.Equals("application/json", StringComparison.OrdinalIgnoreCase) &&
                    _context.HttpContext.Request.ContentLength.HasValue && _context.HttpContext.Request.ContentLength > 0)
            {
                string strJson = string.Empty;
                Type objectType = _context.ActionDescriptor.Parameters
                    .Where(c => !c.ParameterType.IsPrimitive && c.ParameterType != (typeof(string)))
                    .FirstOrDefault()?.ParameterType;
                if (objectType != null)
                {
                    Stream reqBody = _context.HttpContext.Request.Body;
                    reqBody.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(reqBody))
                        strJson = reader.ReadToEnd();
                    if (JSONHelper.IsValidJSON(strJson))
                    {
                        var jsonObject = JsonConvert.DeserializeObject(strJson, objectType);
                        List<string> extraInputs = new List<string>();
                        GetExtraAttributesInJSON(objectType, jsonObject, extraInputs);
                        if (extraInputs != null && extraInputs.Count > 0)
                            lstErrors.Add(string.Format("The following JSON inputs are invalid: '{0}'",
                                String.Join("', '", extraInputs.Select(s => s = AntiXSSHelper.Sanitize(s)))));
                    }
                    else
                    {
                        validContent = false;
                        lstErrors.Add("Please provide a valid JSON content.");
                    }
                }
            }
            return lstErrors;
        }

        private void GetExtraAttributesInJSON(Type classType, object classObject, List<string> extraInputs)
        {
            if (classType.GetInterface(nameof(IEnumerable)) == null)
            {
                PopulateEAFromClassType(classType, classObject, extraInputs);
            }
            else
            {
                if (classObject != null)
                {
                    foreach (var objInCol in ((IEnumerable)classObject))
                    {
                        PopulateEAFromObj(objInCol, extraInputs);
                    }
                }
            }


        }

        private void PopulateEAFromClassType(Type classType, object objInCol, List<string> extraInputs)
        {
            PopoluateExtractInput(objInCol, extraInputs);
            foreach (PropertyInfo property in classType.GetProperties().Where(
                e => (e.PropertyType != typeof(string)) && ((e.PropertyType.IsClass) || (e.PropertyType.IsGenericType
            && e.PropertyType.GetGenericArguments().FirstOrDefault() != typeof(string) && e.PropertyType.GetGenericArguments().First().IsClass)))
                )
            {
                object newClassObject = property.GetValue(objInCol);
                GetExtraAttributesInJSON(property.PropertyType, newClassObject, extraInputs);
            }
        }

        private void PopulateEAFromObj(object objInCol, List<string> extraInputs)
        {
            PopoluateExtractInput(objInCol, extraInputs);
            foreach (PropertyInfo property in objInCol?.GetType().GetProperties().Where(e => (e.PropertyType != typeof(string)) && ((e.PropertyType.IsClass) || (e.PropertyType.IsGenericType && e.PropertyType.GetGenericArguments().FirstOrDefault() != typeof(string) && e.PropertyType.GetGenericArguments().First().IsClass))))
            {
                object newClassObject = property.GetValue(objInCol);
                GetExtraAttributesInJSON(property.PropertyType, newClassObject, extraInputs);
            }
        }
        private void PopoluateExtractInput(object objInCol, List<string> extraInputs)
        {
            PropertyInfo pi = objInCol?.GetType().GetProperty("ExtraAttributes");
            if (pi != null)
            {
                Dictionary<string, object> extraAttributes = (Dictionary<string, object>)(pi.GetValue(objInCol));
                if (extraAttributes != null && extraAttributes.Count > 0)
                {
                    foreach (var ea in extraAttributes)
                        extraInputs.Add(string.Format("{0} : {1}", ea.Key, ea.Value));
                }
            }
        }
    }
}
