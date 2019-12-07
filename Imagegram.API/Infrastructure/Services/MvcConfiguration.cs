using Imagegram.API.Helpers;
using Imagegram.API.Infrastructure.Binders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Infrastructure.Services
{
    public static class MvcConfiguration
    {
        public static void AddRequestQueryBinderProvider(this MvcOptions options)
        {
            var binderToFind = options.ModelBinderProviders.FirstOrDefault(x => x.GetType() == typeof(SimpleTypeModelBinderProvider));
            if (binderToFind == null) return;
            int index = options.ModelBinderProviders.IndexOf(binderToFind);
            options.ModelBinderProviders.Insert(index, new RequestQueryBinderProvider());
            options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((modelValue, modelName) => string.Format("The value '{0}' is not valid for {1}.", AntiXSSHelper.Sanitize(modelValue), modelName));
            options.Filters.Add(new ProducesAttribute("application/json"));
        }
    }
}
