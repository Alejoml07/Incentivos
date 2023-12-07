using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntosLeonisa.Fidelizacion.Domain.Model;

namespace PuntosLeonisa.Infrasctructure.Core.Configuration
{
    public class UsuarioInfoPuntosConfuguration : IEntityTypeConfiguration<UsuarioInfoPuntos>
    {
        public void Configure(EntityTypeBuilder<UsuarioInfoPuntos> builder)
        {
            builder.ToContainer("UsuariosInfoPuntos")
                .HasPartitionKey(e => e.Cedula).HasKey(p=> p.Cedula);
        }

    }
}
