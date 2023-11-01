using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntosLeonisa.Products.Domain.Model;

namespace PuntosLeonisa.Infrasctructure.Core.Configuration
{
    public class UsuariosConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToContainer("Usuario")
                .HasPartitionKey(e => e.Cedula).HasKey(p=> p.Cedula);
        }

    }
}
