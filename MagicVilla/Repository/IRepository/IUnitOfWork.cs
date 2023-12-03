namespace MagicVilla.Repository.IRepository
{
    public interface IUnitOfWork
    {
        public IVillaRepository Villa { get; }
        public IVillaNumberRepository VillaNumber { get; }
        Task SaveAsync();
    }
}
