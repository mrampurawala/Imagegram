using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Imagegram.API.Infrastructure.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IHostingEnvironment _env;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger, IHostingEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception in {_env.ApplicationName}: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            var statusCode = (int)HttpStatusCode.InternalServerError;

            response.ContentType = "application/json";
            response.StatusCode = statusCode;
            if (_env.IsDevelopment())
            {
                await response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    errorMessage = exception.Message,
                    errorStack = exception.StackTrace,
                    innerException = exception.InnerException?.ToString()
                }));
            }
            else if (_env.IsProduction())
            {
                await response.WriteAsync(string.Empty);
            }
            else
            {
                await response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    errorMessage = exception.Message
                }));
            }
        }
    }
}
