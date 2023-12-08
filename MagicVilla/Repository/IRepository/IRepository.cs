using System.Linq.Expressions;

namespace MagicVilla.Repository.IRepository
{
    public interface IRepository<T>
    {
        Task<List<T>> GetAllAsync(bool tracked = true, string? includeProperties = null);
        //IEnumerable<T> GetAllAsync(Func<T, bool> filter);
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, bool tracked = true, string? includeProperties = null);
        Task AddAsync(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> enrtites);
    }
}
