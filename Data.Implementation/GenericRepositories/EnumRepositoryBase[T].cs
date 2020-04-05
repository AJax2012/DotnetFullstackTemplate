using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SourceName.Data.GenericRepositories;
using SourceName.Data.Model;

namespace SourceName.Data.Implementation.GenericRepositories
{
    public class EnumRepositoryBase<TEntity> : IEnumRepository<TEntity> 
        where TEntity : EntityEnum
    {
        protected readonly EntityContext _context;

        public EnumRepositoryBase(EntityContext context)
        {
            _context = context;
        }

        public virtual TEntity GetById(int id)
        {
            return _context.Set<TEntity>().Single(x => x.Id == id);
        }

        public virtual TEntity GetByName(string name)
        {
            return _context.Set<TEntity>().Single(x => x.Name == name);
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null)
        {
            var query = _context.Set<TEntity>().AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.AsEnumerable();
        }
    }
}