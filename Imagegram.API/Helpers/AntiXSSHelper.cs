using Ganss.XSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Helpers
{
    public static class AntiXSSHelper
    {
        public static string Sanitize(string input)
        {
            var sanitizer = new HtmlSanitizer() { KeepChildNodes = true };
            return sanitizer.Sanitize(input);
        }
    }
}
