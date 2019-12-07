using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Infrastructure.Middlewares
{
    public class RequestHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IRequestHeaders headers)
        {
            IHeaderDictionary dictHeaders = context.Request.Headers;

            if (dictHeaders.ContainsKey("X-Account-Id"))
            {
                headers.UUID = dictHeaders["X-Account-Id"];
            }
            else
            {

                context.Response.StatusCode = 401; //Bad Request                
                await context.Response.WriteAsync("API Key is missing");
                return;
            }

            await _next(context);
        }
    }
}
