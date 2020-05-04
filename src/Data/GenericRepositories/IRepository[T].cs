using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SourceName.Data.GenericRepositories
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null);
        TEntity Insert(TEntity entity);
        TEntity Update(TEntity entity);
    }
}