using MagicVilla.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
            
        }

        public DbSet<VillaDTO> Villas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VillaDTO>().HasData(
                new VillaDTO { Id = 1, Name = "ALAYA BARBADOS", Occupancy = 5, SquareMeters = 250 },
                new VillaDTO { Id = 2, Name = "VILLA RICA IBIZA", Occupancy = 3, SquareMeters = 120 },
                new VillaDTO { Id = 3, Name = "CASA TRES SOLES", Occupancy = 1, SquareMeters = 60 },
                new VillaDTO { Id = 4, Name = "LA BERGERIE", Occupancy = 2, SquareMeters = 100 }
                );
        }
    }
}
