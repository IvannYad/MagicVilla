namespace MagicVilla.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public IVillaRepository Villa { get; }
        Task SaveAsync();
    }
}
