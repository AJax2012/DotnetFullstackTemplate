using System.Collections.Generic;

namespace SourceName.Data.Model
{
    public class PaginatedResult<TEntity> where TEntity : class
    {
        public IEnumerable<TEntity> Data { get; set; }
        public int TotalCount { get; set; }
    }
}
