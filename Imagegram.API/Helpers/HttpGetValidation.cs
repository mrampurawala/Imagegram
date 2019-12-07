using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Imagegram.API.Helpers
{
    public class HTTPGetValidation : ContextValidation
    {
        public HTTPGetValidation(ActionExecutingContext context) : base(context)
        {
        }
        public override List<string> getErrorList()
        {
            if (_context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                var queryParams = _context.HttpContext.Request.Query;
                if (queryParams.Count > 0)
                {
                    List<string> validParams = new List<string>();
                    var lstParamDesc = _context.ActionDescriptor.Parameters;
                    if (lstParamDesc != null && lstParamDesc.Count > 0)
                    {
                        foreach (ParameterDescriptor paramDesc in lstParamDesc)
                            if (!paramDesc.ParameterType.IsPrimitive && paramDesc.ParameterType != (typeof(string)))
                                foreach (PropertyInfo prop in paramDesc.ParameterType.GetProperties())
                                    validParams.Add(prop.Name);
                    }
                    var lstInvalidParams = queryParams.Where(q => !validParams.Any(p => p.Equals(q.Key, StringComparison.OrdinalIgnoreCase))).Select(r => r.Key);
                    if (lstInvalidParams != null && Enumerable.Count(lstInvalidParams) > 0)
                        lstErrors.Add(string.Format("The following input parameters are invalid: '{0}'", String.Join("', '", lstInvalidParams.Select(s => s = AntiXSSHelper.Sanitize(s)))));
                }
            }
            return lstErrors;
        }
    }
}
