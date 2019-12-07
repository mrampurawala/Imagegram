using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Infrastructure.Middlewares
{
    public class RequestHeaders : IRequestHeaders
    {
        public string UUID { get; set; }
    }

    public interface IRequestHeaders
    {
        string UUID { get; set; }
    }
}
