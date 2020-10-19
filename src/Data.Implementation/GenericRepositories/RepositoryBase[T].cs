using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        public virtual async Task<TEntity> GetEntityAsync(Expression<Func<TEntity, bool>> where = null, bool trackEntity = true)
        {
            var entity = await _context.Set<TEntity>().FirstOrDefaultAsync(where);
            if (entity != null)
            {
                if (!trackEntity)
                {
                    _context.Entry(entity).State = EntityState.Detached;
                }
            }
            return entity;
        }

        public virtual async Task<TEntity> GetEntityFirstOrDefaultAsync(Query<TEntity> request)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (request.IncludeProperties != null)
            {
                foreach (var prop in request.IncludeProperties)
                {
                    query = query.Include(prop);
                }
            }

            return await query.FirstOrDefaultAsync(request.Where);
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}