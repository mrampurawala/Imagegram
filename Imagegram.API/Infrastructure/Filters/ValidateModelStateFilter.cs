using Imagegram.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Infrastructure.Filters
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            List<string> lstErrors = new List<string>();
            bool isContentValid = true;
            if (context.HttpContext.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                ContextValidation gv = new HTTPGetValidation(context);
                lstErrors = gv.getErrorList();
            }
            else if (context.HttpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
            {
                ContextValidation pv = new HTTPPostValidation(context);
                lstErrors = pv.getErrorList();
                isContentValid = pv.validContent;
            }

            if (isContentValid && !context.ModelState.IsValid)
                lstErrors.AddRange((from modelState in context.ModelState.Values from error in modelState.Errors select error.ErrorMessage).ToList());

            if (lstErrors.Count > 0)
                context.Result = new BadRequestObjectResult(lstErrors);

            base.OnActionExecuting(context);
        }

    }
}
