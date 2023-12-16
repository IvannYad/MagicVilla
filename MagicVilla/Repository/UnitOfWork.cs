using MagicVilla.Data;
using MagicVilla.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace MagicVilla.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context, [FromServices] IConfiguration configuration)
        {
            _context = context;
            Villa = new VillaRepository(_context);
            VillaNumber = new VillaNumberRepository(_context);
            User = new UserRepository(_context, configuration);
        }
        public IVillaRepository Villa { get; private set; }
        public IVillaNumberRepository VillaNumber { get; private set; }
        public IUserRepository User { get; private set; }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
