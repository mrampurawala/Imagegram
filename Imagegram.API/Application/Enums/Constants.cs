using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Application.Enums
{
    public class Constants
    {
        public enum ImageFormat
        {
            [Description("JPG")]
            JPG = 1,
            [Description("BMP")]
            BMP = 2,
            [Description("PNG")]
            PNG = 3
        }
    }
}
