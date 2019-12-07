using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Helpers
{
    public class ContextValidation
    {
        public bool validContent { get; set; } = true;
        protected ActionExecutingContext _context;
        protected List<string> lstErrors = new List<string>();
        public ContextValidation(ActionExecutingContext context)
        {
            this._context = context;
        }
        public virtual List<string> getErrorList()
        {
            return lstErrors;
        }
    }
}
