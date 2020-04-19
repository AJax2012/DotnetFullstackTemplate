using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SourceName.Data.Implementation.Role;
using SourceName.Data.Implementation.User;
using SourceName.Data.Model;
using SourceName.Data.Roles;
using SourceName.Data.Users;

namespace SourceName.DependencyInjection
{
    public static class DataLayerDependencyInjection
    {
        public static void AddDataLayer(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<EntityContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
        }
    }
}