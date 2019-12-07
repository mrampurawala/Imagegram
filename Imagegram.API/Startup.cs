using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using Imagegram.API.Helpers;
using Imagegram.API.Infrastructure.Filters;
using Imagegram.API.Infrastructure.Middlewares;
using Imagegram.API.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace Imagegram.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(options => { options.Filters.Add(new ValidateModelStateFilter()); })
                .AddFluentValidation(options => { options.RegisterValidatorsFromAssemblyContaining<Startup>(); })
                .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services
                .ConfigureRepositories(Configuration)
                .AddMiddleware()
                .AddCorsConfiguration()
                .AddRequestHeaders()
                .AddPostImageSettings(Configuration)
                .AddConnectionSettings(Configuration);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("imagegram", new Info { Title = "Imagegram API", Version = "v1" });
                c.DescribeAllEnumsAsStrings();
                c.OperationFilter<FileUploadOperation>(); //Register File Upload Operation Filter
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddMvc(o => o.AddRequestQueryBinderProvider());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/imagegram/swagger.json", "Imagegram V1");
            });
            app
            .UseRequestHeaders()
            .UseCustomExceptionHandler();
            //app.UseAuthentication();
            app.UseMvc();
        }
    }
}
