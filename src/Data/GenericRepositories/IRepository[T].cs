using System.Collections.Generic;
using System.Threading.Tasks;
using SourceName.Data.Model;

namespace SourceName.Data.GenericRepositories
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        IEnumerable<TEntity> GetEntities(Query<TEntity> request);
        Task<TEntity> GetEntityAsync(System.Linq.Expressions.Expression<System.Func<TEntity, bool>> where = null, bool trackEntity = true);
        Task<TEntity> GetEntityFirstOrDefaultAsync(Query<TEntity> request);
        Task<PaginatedResult<TEntity>> GetPaginatedEntitiesAsync(PagingatedQuery<TEntity> request);
        Task<TEntity> InsertAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
    }
}