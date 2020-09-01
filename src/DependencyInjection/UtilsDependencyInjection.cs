using Microsoft.Extensions.DependencyInjection;
using SourceName.Utils.Implementations;
using SourceName.Utils.Interfaces;

namespace SourceName.DependencyInjection
{
    public static class UtilsDependencyInjection
    {
        public static void AddUtils(this IServiceCollection services)
        {
            services.AddSingleton<IDateTime, DateTimeProvider>();
        }
    }
}
