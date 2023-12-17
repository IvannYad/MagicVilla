using MagicVilla.Data;
using MagicVilla.Models;
using MagicVilla.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace MagicVilla.Repository
{
    public class Repository<T> : IRepository<T>
        where T : class
    {
        private readonly DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        {
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<List<T>> GetAllAsync(bool tracked = true
            , string? includeProperties = null
            , int pageSize = 0
            , int pageNumber = 1)
        {
            IQueryable<T> query = _dbSet;
            if (!tracked)
                query = query.AsNoTracking();

            if (pageSize > 0)
            {
                if (pageSize > 100)
                    pageSize = 100;

                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }

            if (includeProperties is not null)
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, bool tracked = true, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;
            if (!tracked)
                query = query.AsNoTracking();

            query = query.Where(filter);
            if (includeProperties is not null)
            {
                foreach (var includeProperty in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
