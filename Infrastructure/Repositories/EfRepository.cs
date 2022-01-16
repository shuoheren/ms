using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ApplicationCore.Helpers;
using ApplicationCore.RepositoryInterfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EfRepository<T> : IAsyncRepository<T> where T : class
    {
        protected readonly MovieShopDbContext movieShopDbContext;

        public EfRepository(MovieShopDbContext dbContext)
        {
            movieShopDbContext = dbContext;
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await movieShopDbContext.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> ListAllAsync()
        {
            return await movieShopDbContext.Set<T>().ToListAsync();
        }

        public async Task<IEnumerable<T>> ListAllWithIncludesAsync(Expression<Func<T, bool>> where,
            params Expression<Func<T, object>>[] includes)
        {
            var query = movieShopDbContext.Set<T>().AsQueryable();

            if (includes != null)
                foreach (var navigationProperty in includes)
                    query = query.Include(navigationProperty);

            return await query.Where(where).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> filter)
        {
            return await movieShopDbContext.Set<T>().Where(filter).ToListAsync();
        }

        public virtual async Task<int> GetCountAsync(Expression<Func<T, bool>> filter = null)
        {
            if (filter != null)
                return await movieShopDbContext.Set<T>().Where(filter).CountAsync();
            return await movieShopDbContext.Set<T>().CountAsync();
        }

        public virtual async Task<bool> GetExistsAsync(Expression<Func<T, bool>> filter = null)
        {
            if (filter == null) return false;
            return await movieShopDbContext.Set<T>().Where(filter).AnyAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            movieShopDbContext.Set<T>().Add(entity);
            await movieShopDbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            movieShopDbContext.Entry(entity).State = EntityState.Modified;
            await movieShopDbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task DeleteAsync(T entity)
        {
            movieShopDbContext.Set<T>().Remove(entity);
            await movieShopDbContext.SaveChangesAsync();
        }

        public virtual async Task<PaginatedList<T>> GetPagedData(int page = 1, int pageSize = 25,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderedQuery
                = null, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes)
        {
            var pagedList =
                await PaginatedList<T>.GetPaged(movieShopDbContext.Set<T>(), page, pageSize, orderedQuery, filter, includes);
            return pagedList;
        }
    }
}