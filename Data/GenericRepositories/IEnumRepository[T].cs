using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SourceName.Data.Model;

namespace SourceName.Data.GenericRepositories
{
    public interface IEnumRepository<TEntity> where TEntity : EntityEnum
    {
        TEntity GetById(int id);
        TEntity GetByName(string name);
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null);
    }
}