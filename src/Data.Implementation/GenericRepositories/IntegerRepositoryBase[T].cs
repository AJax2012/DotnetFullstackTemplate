using System.Linq;
using System.Threading.Tasks;
using SourceName.Data.GenericRepositories;
using SourceName.Data.Model;

namespace SourceName.Data.Implementation.GenericRepositories
{
    public abstract class IntegerRepositoryBase<TEntity> : RepositoryBase<TEntity>, IIntegerRepository<TEntity>
        where TEntity : EntityWithIntegerId
    {
        protected IntegerRepositoryBase(EntityContext context) : base(context) {}

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);

            if (entity == null)
            {
                return false;
            }

            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();

            return true;
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }
    }
}