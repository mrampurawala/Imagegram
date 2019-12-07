using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Application.Validations
{
    /// <summary>
    /// 
    /// </summary>
    public class SearchBase
    {
        /// <summary>
        /// Page index (optional), default value is 1
        /// </summary>
        public int page { get; set; } = 1;
        /// <summary>
        /// Page size (optional), default value is 20. Maximum value is 50.
        /// </summary>
        public int limit { get; set; } = 20;
    }
}
