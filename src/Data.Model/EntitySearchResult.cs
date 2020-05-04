using System.Collections.Generic;

namespace SourceName.Data.Model
{
    public class EntitySearchResult<TEntity>
    {
        public int TotalCount { get; set; }
        public IEnumerable<TEntity> Results { get; set; }
    }
}