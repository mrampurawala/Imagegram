using Imagegram.API.Application.Repositories;
using Imagegram.API.Infrastructure.Configurations;
using Imagegram.API.Infrastructure.Middlewares;
using Imagegram.API.Infrastructure.Repositories.Dapper;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Imagegram.API.Infrastructure.Services
{
    public static class ServicesConfiguration
    {
        public static IServiceCollection ConfigureRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAccountRepository, AccountRepository>()
                .AddScoped<IPostRepository, PostRepository>()
                ;

            return services;
        }

        //public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        //{
        //    //services.AddSingleton(configuration.GetSection("AppSettings").Get<AppSettings>());

        //    //return services;
        //}

        public static IServiceCollection AddConnectionSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration.GetSection("ConnectionStrings").Get<ConnectionStrings>());
            return services;
        }

        public static IServiceCollection AddPostImageSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration.GetSection("PostImage").Get<ImageContentType>());
            return services;
        }

        public static IServiceCollection AddRequestHeaders(this IServiceCollection services)
        {
            services.AddScoped<IRequestHeaders, RequestHeaders>();

            return services;
        }

        public static IServiceCollection AddMiddleware(this IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            return services;
        }

        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", new CorsPolicyBuilder()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials()
                    .Build());
            });

            return services;
        }
    }
}
