using Microsoft.Extensions.DependencyInjection;
using SourceName.Service.Implementation.Init;
using SourceName.Service.Implementation.Users;
using SourceName.Service.Init;
using SourceName.Service.Users;

namespace SourceName.DependencyInjection
{
    public static class ServiceLayerDependencyInjection
    {
        public static void AddServiceLayer(this IServiceCollection services)
        {
            services.AddScoped<IInitialSetupService, InitialSetupService>();
            services.AddScoped<IUserCapabilitiesService, UserCapabilitiesService>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IUserPasswordService, UserPasswordService>();
            services.AddScoped<IUserPasswordValidationService, UserPasswordValidationService>();
            services.AddScoped<IUserValidationService, UserValidationService>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}