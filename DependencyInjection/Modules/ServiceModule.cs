using Microsoft.Extensions.DependencyInjection;
using SourceName.Service.Implementation.Users;
using SourceName.Service.Users;

namespace SourceName.DependencyInjection.Modules
{
    public class ServiceModule : IDependencyInjectionModule
    {
        public void RegisterDependencies(IServiceCollection services)
        {
            services.AddScoped<IUserCapabilitiesService, UserCapabilitiesService>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IUserPasswordService, UserPasswordService>();
            services.AddScoped<IUserService, UserService>();
        }
    }
}