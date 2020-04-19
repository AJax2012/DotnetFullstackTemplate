using AutoMapper;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using SourceName.DependencyInjection;
using SourceName.DependencyInjection.Modules;
using SourceName.Mapping;
using SourceName.Service.Init;
using SourceName.Api.Core.Authentication;
using SourceName.Api.Model.Configuration;
using SourceName.Api.Core.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SourceName.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //var startupClass = typeof(Startup);
            //var fullNamespace = startupClass.Namespace;
            //Namespace = fullNamespace?.Substring(0, fullNamespace.IndexOf("."));
        }

        public IConfiguration Configuration { get; }
        //public string Namespace { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SecretsConfiguration>(Configuration.GetSection("Secrets"));

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(UserContextFilter));
            });

            services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy", policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });

            services.AddAutoMapper(typeof(ApiMappingProfile).Assembly);

            var userPasswordSecret = Configuration.GetSection("Secrets").GetValue("UserPasswordSecret", "").ToString();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromMinutes(2),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(userPasswordSecret)),
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            new List<IDependencyInjectionModule>
            {
                new ServiceModule(),
                new DataModule(Configuration.GetConnectionString("SourceNameDatabase"))
            }.ForEach(module => module.RegisterDependencies(services));

            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
            services.AddScoped<UserContextFilter>();

            // get namespace to string
            var apiInfo = new OpenApiInfo
            {
                Title = "SourceName",
                Version = "v1"
            };

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", apiInfo);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IInitialSetupService initialSetupService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); 
                
                app.UseSwagger();
                
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SourceName V1");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            initialSetupService.InitialSetup();
        }
    }
}
