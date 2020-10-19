using SourceName.Data.Model;
using System.Threading.Tasks;

namespace SourceName.Data.GenericRepositories
{
    public interface IIntegerRepository<TEntity> where TEntity : EntityWithIntegerId
    {
        Task<bool> DeleteAsync(int id);
        Task<TEntity> GetByIdAsync(int id);
    }
}