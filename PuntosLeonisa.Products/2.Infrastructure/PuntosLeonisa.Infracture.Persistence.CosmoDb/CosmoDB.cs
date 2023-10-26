using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Infrasctructure.Core.Configuration;
using PuntosLeonisa.Products.Domain;

namespace PuntosLeonisa.infrastructure.Persistence.CosmoDb;


public class CosmoDB : DbContext
{
    //public DbSet<Producto>? Productos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.UseCosmos(
        //    "https://adminbd.documents.azure.com:443/",
        //    "CksJmbXM8eBepSYgTYRbXKRRDguumy8hp3vnOIiKprPyuZ9zWBYtv4iB54oD8JpPLRbM2l22zrDshACDbzjm6Og==",
        //    "puntosleonisa_dllo"
        //    );

        optionsBuilder.UseCosmos(
            "AccountEndpoint=https://adminbd.documents.azure.com:443/;AccountKey=ksJmbXM8eBepSYgTYRbXKRRDguumy8hp3vnOIiKprPyuZ9zWBYtv4iB54oD8JpPLRbM2l22zrDshACDbzjm6Og==;",
            "puntosleonisa_dllo"
            );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductosConfiguration());
        modelBuilder.ApplyConfiguration(new UsuariosConfiguration());

    }

}


