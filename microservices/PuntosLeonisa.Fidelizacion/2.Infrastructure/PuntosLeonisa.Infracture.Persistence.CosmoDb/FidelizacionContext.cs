using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Infrasctructure.Core.Configuration;
using PuntosLeonisa.infrastructure.Persistence.Configuration;

namespace PuntosLeonisa.infrastructure.Persistence.CosmoDb;


public class FidelizacionContext : DbContext, IDisposable
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
        modelBuilder.ApplyConfiguration(new MovimientoPuntosConfiguration());
        modelBuilder.ApplyConfiguration(new WishListConfiguration());
        modelBuilder.ApplyConfiguration(new CarritoConfiguration());
        modelBuilder.ApplyConfiguration(new UsuarioInfoPuntosConfuguration());
        modelBuilder.ApplyConfiguration(new SmsConfiguration());
        modelBuilder.ApplyConfiguration(new UsuarioRedencionConfiguration());
        modelBuilder.ApplyConfiguration(new ExtractoConfiguration());
        modelBuilder.ApplyConfiguration(new FidelizacionPuntosConfiguration());

    }

}


