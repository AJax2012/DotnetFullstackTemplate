using Microsoft.Extensions.DependencyInjection;

namespace SourceName.DependencyInjection
{
    public interface IDependencyInjectionModule
    {
        void RegisterDependencies(IServiceCollection services);
    }
}