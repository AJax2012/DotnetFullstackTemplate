using System;
using System.Linq;
using SourceName.Data.GenericRepositories;
using SourceName.Data.Model;

namespace SourceName.Data.Implementation.GenericRepositories
{
    public abstract class GuidRepositoryBase<TEntity> : RepositoryBase<TEntity>, IGuidRepository<TEntity>
        where TEntity : EntityWithGuidId
    {
        protected GuidRepositoryBase(EntityContext context) : base(context) {}

        public virtual void Delete(Guid id)
        {
            var entity = _context.Set<TEntity>().Single(x => x.Id == id);
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
        }

        public virtual TEntity GetById(Guid id)
        {
            return _context.Set<TEntity>().SingleOrDefault(x => x.Id == id);
        }
    }
}