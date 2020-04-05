using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SourceName.Data.Implementation.User;
using SourceName.Data.Model;
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
            services.AddDbContext<EntityContext>(options => options.UseSqlServer(_connectionString));

            services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}