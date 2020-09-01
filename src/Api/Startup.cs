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
using SourceName.Mapping;
using SourceName.Service.Init;
using SourceName.Api.Core.Authentication;
using SourceName.Api.Core.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SourceName.Utils.Configuration;

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
            services.Configure<AppConfig>(Configuration.GetSection("AppConfig"));

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

            services.AddUtils();
            services.AddServiceLayer();
            services.AddDataLayer(Configuration.GetConnectionString("SourceNameDatabase"));

            services.AddScoped<IUserAuthenticationService, UserAuthenticationService>();
            services.AddScoped<UserContextFilter>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SourceName",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
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
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            initialSetupService.InitialSetup();
        }
    }
}
