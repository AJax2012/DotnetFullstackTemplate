using SourceName.Data.Model;

namespace SourceName.Data.GenericRepositories
{
    public interface IIntegerRepository<TEntity> where TEntity : EntityWithIntegerId
    {
        void Delete(int id);
        TEntity GetById(int id);
    }
}