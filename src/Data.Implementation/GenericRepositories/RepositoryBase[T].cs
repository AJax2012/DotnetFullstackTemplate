using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SourceName.Data.GenericRepositories;
using SourceName.Data.Model;

namespace SourceName.Data.Implementation.GenericRepositories
{
    public class RepositoryBase<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected readonly EntityContext _context;

        public RepositoryBase(EntityContext context)
        {
            _context = context;
        }

        public virtual IEnumerable<TEntity> GetEntities(Query<TEntity> request)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            // Apply where.
            if (request.Where != null)
            {
                query = query.Where(request.Where);
            }

            // Count.
            int count = query.Count();

            // Include.
            if (request.IncludeProperties != null)
            {
                foreach (var prop in request.IncludeProperties)
                {
                    query = query.Include(prop);
                }
            }

            // Apply order.
            if (request.OrderPrimary != null)
            {
                if (!request.OrderByAscending)
                {
                    query = request.OrderSecondary != null
                        ? query.OrderByDescending(request.OrderPrimary).ThenByDescending(request.OrderSecondary)
                        : query.OrderByDescending(request.OrderPrimary);
                }
                else
                {
                    query = request.OrderSecondary != null
                        ? query.OrderBy(request.OrderPrimary).ThenBy(request.OrderSecondary)
                        : query.OrderBy(request.OrderPrimary);
                }
            }
            else
            {
                // use reflection to get key
                Type type = typeof(TEntity);
                var key = type.GetProperties().FirstOrDefault(p =>
                    p.CustomAttributes.Any(attr =>
                        attr.AttributeType == typeof(KeyAttribute)));
                query = query.OrderBy(e => key.Name);
            }

            var data = request.TrackEntities ? query : query.AsNoTracking();

            return data;
        }

        public virtual async Task<PaginatedResult<TEntity>> GetPaginatedEntitiesAsync(PagingatedQuery<TEntity> request)
        {
            var unpaginatedRequest = request.ToBaseQuery();
            IQueryable<TEntity> query = GetEntities(unpaginatedRequest).AsQueryable();

            query = query
                .Skip(request.StartNumber)
                .Take(request.ResultsPerPage);

            return new PaginatedResult<TEntity>()
            {
                Data = await query.ToListAsync(),
                TotalCount = query.Count()
            };
        }

        public async Task<TEntity> GetEntityAsync(object id, bool trackEntity = true)
        {
            var entity = await _context.FindAsync(typeof(TEntity), id);
            if (entity != null)
            {
                if (!trackEntity)
                {
                    _context.Entry(entity).State = EntityState.Detached;
                }
                return (TEntity)entity;
            }
            return null;
        }

        public async Task<TEntity> GetEntityFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, IList<Expression<Func<TEntity, object>>> includeProperties = null)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (includeProperties != null)
            {
                foreach (var prop in includeProperties)
                {
                    query = query.Include(prop);
                }
            }

            return await query.FirstOrDefaultAsync(filter);
        }

        public virtual TEntity Insert(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            _context.SaveChanges();
            return entity;
        }

        protected virtual List<TEntity> ApplyPagingAndSorting(
            List<TEntity> baseQuery,
            int? limit,
            int? offset,
            string orderBy,
            string orderDirection,
            Dictionary<string, Func<TEntity, object>> orderBySelectors)
        {
            var query = baseQuery;
            if (!string.IsNullOrWhiteSpace(orderBy) && orderBySelectors.ContainsKey(orderBy))
            {
                var orderBySelector = orderBySelectors[orderBy];
                if (orderDirection.Equals("asc", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.OrderBy(orderBySelector).ToList();
                }
                else
                {
                    query = query.OrderByDescending(orderBySelector).ToList();
                }
            }

            if (offset.HasValue)
            {
                query = query.Skip(offset.Value).ToList();
            }
            if (limit.HasValue)
            {
                query = query.Take(limit.Value).ToList();
            }

            return query;
        }
    }
}