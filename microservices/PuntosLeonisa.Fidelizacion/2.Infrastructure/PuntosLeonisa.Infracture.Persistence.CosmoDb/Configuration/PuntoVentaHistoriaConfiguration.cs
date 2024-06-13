using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Model;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class PuntoVentaHistoriaConfiguration : IEntityTypeConfiguration<PuntoVentaHistoria>
    {
        public void Configure(EntityTypeBuilder<PuntoVentaHistoria> builder)
        {
            builder.ToContainer("PuntoVentaHistoria")
                .HasPartitionKey(e => e.Id);
        }

    }
}
