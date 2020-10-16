using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SourceName.Data.Model
{
    public class Query<TEntity> where TEntity : class
    {
        public bool OrderByAscending { get; set; } = true;
        public bool TrackEntities { get; set; } = false;
        public Expression<Func<TEntity, bool>> Where { get; set; } = null;
        public Expression<Func<TEntity, object>> OrderPrimary { get; set; } = null;
        public Expression<Func<TEntity, object>> OrderSecondary { get; set; } = null;
        public IEnumerable<Expression<Func<TEntity, object>>> IncludeProperties { get; set; } = new List<Expression<Func<TEntity, object>>>();
    }

    public class PagingatedQuery<TEntity> : Query<TEntity> 
        where TEntity : class 
    {
        public int StartNumber { get; private set; }
        public int ResultsPerPage { get; private set; }

        public PagingatedQuery(int pageNumber = 0, int resultsPerPage = 10)
        {
            ResultsPerPage = resultsPerPage;
            StartNumber = pageNumber * resultsPerPage;
        }

        public Query<TEntity> ToBaseQuery()
        {
            return new Query<TEntity>
            {
                OrderByAscending = OrderByAscending,
                TrackEntities = TrackEntities,
                Where = Where,
                OrderPrimary = OrderPrimary,
                OrderSecondary = OrderSecondary,
                IncludeProperties = IncludeProperties
            };
        }
    }
}
