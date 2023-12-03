using System.Linq.Expressions;

namespace MagicVilla.Repository.IRepository
{
    public interface IRepository<T>
    {
        Task<List<T>> GetAllAsync();
        //IEnumerable<T> GetAllAsync(Func<T, bool> filter);
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, bool tracked = false);
        Task CreateAsync(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> enrtites);
    }
}
