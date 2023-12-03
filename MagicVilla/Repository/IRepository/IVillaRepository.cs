using MagicVilla.Models;

namespace MagicVilla.Repository.IRepository
{
    public interface IVillaRepository : IRepository<Villa>
    {
        void Update(Villa entity);
    }
}
