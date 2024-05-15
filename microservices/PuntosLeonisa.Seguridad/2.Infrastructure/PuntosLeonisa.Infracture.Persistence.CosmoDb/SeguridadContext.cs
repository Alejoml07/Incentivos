using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Infrasctructure.Core.Configuration;
using PuntosLeonisa.infrastructure.Persistence.Configuration;

namespace PuntosLeonisa.infrastructure.Persistence.CosmoDb;


public class SeguridadContext : DbContext
{
    public SeguridadContext()
    {

    }
    public SeguridadContext(DbContextOptions<SeguridadContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseCosmos(
        //    "https://adminbd.documents.azure.com:443/",
        //    "CksJmbXM8eBepSYgTYRbXKRRDguumy8hp3vnOIiKprPyuZ9zWBYtv4iB54oD8JpPLRbM2l22zrDshACDbzjm6Og==",
        //    "puntosleonisa_dllo"
        //    );

        //optionsBuilder.UseCosmos(
        //    "AccountEndpoint=https://adminbd.documents.azure.com:443/;AccountKey=ksJmbXM8eBepSYgTYRbXKRRDguumy8hp3vnOIiKprPyuZ9zWBYtv4iB54oD8JpPLRbM2l22zrDshACDbzjm6Og==;",
        //    "puntosleonisa_dllo"
        //    );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UsuariosConfiguration());
        modelBuilder.ApplyConfiguration(new ProveedorConfiguration());
        modelBuilder.ApplyConfiguration(new TokenConfiguration());
        modelBuilder.ApplyConfiguration(new TratamientoDatosConfiguration());

    }

}


