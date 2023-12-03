using MagicVilla.Models;

namespace MagicVilla.Repository.IRepository
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
        void Update(VillaNumber entity);
    }
}
