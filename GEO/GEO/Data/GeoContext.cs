using GEO.Models;
using Microsoft.EntityFrameworkCore;

namespace GEO.Data
{
    public class GeoContext : DbContext
    {
        public GeoContext(DbContextOptions<GeoContext> options) : base(options)
        {
        }

        public DbSet<Direccion> Direcciones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Direccion>().ToTable("Direcciones");
        }

    }
}