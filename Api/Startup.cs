using AutoMapper;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

using SourceName.DependencyInjection;
using SourceName.DependencyInjection.Modules;
using SourceName.Mapping;

namespace SourceName.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var startupClass = typeof(Startup);
            var fullNamespace = startupClass.Namespace;
            Namespace = fullNamespace?.Substring(0, fullNamespace.IndexOf("."));
        }

        public IConfiguration Configuration { get; }
        public string Namespace { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddAutoMapper(typeof(ApiMappingProfile).Assembly, typeof(ServiceMappingProfile).Assembly);

            new List<IDependencyInjectionModule>
            {
                new ServiceModule(),
                new DataModule(Configuration.GetConnectionString("SourceNameDatabase"))
            }.ForEach(module => module.RegisterDependencies(services));


            // get namespace to string
            var apiInfo = new OpenApiInfo
            {
                Title = Namespace,
                Version = "v1"
            };

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", apiInfo);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); 
                
                app.UseSwagger();
                
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Namespace} V1");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
