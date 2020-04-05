using System;
using SourceName.Data.Model;

namespace SourceName.Data.GenericRepositories
{
    public interface IGuidRepository<TEntity> where TEntity : EntityWithGuidId
    {
        void Delete(Guid id);
        TEntity GetById(Guid id);
    }
}