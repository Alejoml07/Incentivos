using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Infrasctructure.Core.Configuration;
using PuntosLeonisa.infrastructure.Persistence.Configuration;

namespace PuntosLeonisa.infrastructure.Persistence.CosmoDb;


public class FidelizacionContext : DbContext
{
    public FidelizacionContext()
    {

    }
    public FidelizacionContext(DbContextOptions<FidelizacionContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new PuntosManualConfiguration());
        modelBuilder.ApplyConfiguration(new WishListConfiguration());
        modelBuilder.ApplyConfiguration(new CarritoConfiguration());
        modelBuilder.ApplyConfiguration(new UsuarioInfoPuntosConfuguration());

    }

}


