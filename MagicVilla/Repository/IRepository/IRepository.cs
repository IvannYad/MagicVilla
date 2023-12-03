namespace MagicVilla.Repository.IRepository
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAllAsync();
        //IEnumerable<T> GetAllAsync(Func<T, bool> filter);
        T GetAsync(Func<T, bool> filter, bool tracked = false);
        Task CreateAsync(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> enrtites);
    }
}
