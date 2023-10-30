using System;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntosLeonisa.Products.Domain;

namespace PuntosLeonisa.Infrasctructure.Core.Configuration
{
    public class ProductosConfiguration : IEntityTypeConfiguration<Producto>
    {
        public void Configure(EntityTypeBuilder<Producto> builder)
        {
            builder.ToContainer("Producto")
                .HasPartitionKey(e => e.EAN).HasKey(e=> e.EAN);
        }
    }
}

