using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Infrastructure.Binders
{
    public class RequestQueryBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (!context.Metadata.IsComplexType && (context.Metadata.ModelType == typeof(string) ||
                context.Metadata.ModelType == typeof(bool?) ||
                context.Metadata.ModelType == typeof(bool)))
            {
                var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
                return new RequestQueryBinder(new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory));
            }
            return null;
        }
    }
}
