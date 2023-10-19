using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Products.Domain;

namespace PuntosLeonisa.infrastructure.Persistence.CosmoDb;


public class CosmoDB : DbContext
{
    public DbSet<Productos>? Productos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseCosmos(
            "https://localhost:8081",
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
            "Productos-db"
            );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Productos>()
            .ToContainer("Productos")
            .HasPartitionKey(e => e.Id);

        modelBuilder.Entity<Productos>().OwnsOne(p => p.Nombre);
    }

}


