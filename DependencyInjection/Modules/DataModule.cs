using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SourceName.Data.GenericRepositories;
using SourceName.Data.Implementation.GenericRepositories;
using SourceName.Data.Implementation.User;
using SourceName.Data.Model.Role;
using SourceName.Data.Users;

namespace SourceName.DependencyInjection.Modules
{
    public class DataModule : IDependencyInjectionModule
    {
        private readonly string _connectionString;

        public DataModule(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void RegisterDependencies(IServiceCollection services)
        {
            // services.AddDbContext<SourceNameContext>(options => options.UseSqlServer(_connectionString));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEnumRepository<RoleEntity>, EnumRepositoryBase<RoleEntity>>();
        }
    }
}