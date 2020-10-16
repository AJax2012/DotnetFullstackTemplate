using System.Collections.Generic;
using System.Threading.Tasks;
using SourceName.Data.Model;

namespace SourceName.Data.GenericRepositories
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        IEnumerable<TEntity> GetEntities(Query<TEntity> request);
        Task<PaginatedResult<TEntity>> GetPaginatedEntitiesAsync(PagingatedQuery<TEntity> request);
        TEntity Insert(TEntity entity);
        TEntity Update(TEntity entity);
    }
}