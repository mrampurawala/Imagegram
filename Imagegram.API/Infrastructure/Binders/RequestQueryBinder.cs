using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace Imagegram.API.Infrastructure.Binders
{
    public class RequestQueryBinder : IModelBinder
    {
        private readonly IModelBinder FallbackBinder;
        public RequestQueryBinder(IModelBinder fallbackBinder)
        {
            FallbackBinder = fallbackBinder ?? throw new ArgumentNullException(nameof(fallbackBinder));
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (bindingContext.ModelType == typeof(string))
            {
                if (valueProviderResult != null && valueProviderResult.FirstValue is string str && !string.IsNullOrEmpty(str))
                {
                    bindingContext.Result = ModelBindingResult.Success(str.Trim());
                    return Task.CompletedTask;
                }
            }
            else if (bindingContext.ModelType == typeof(bool?) || bindingContext.ModelType == typeof(bool))
            {
                if (valueProviderResult != null && valueProviderResult.FirstValue is string str && str != null && str.Trim() == string.Empty)
                {
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, string.Format("The value '' is not valid for {0}.", bindingContext.ModelName));
                    return Task.CompletedTask;
                }
            }
            return FallbackBinder.BindModelAsync(bindingContext);
        }
    }
}
