using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Model.Carrito;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class PuntoDeVentaConfiguration : IEntityTypeConfiguration<PuntoDeVenta>
    {
        public void Configure(EntityTypeBuilder<PuntoDeVenta> builder)
        {
            builder.ToContainer("PuntoDeVenta")
                .HasPartitionKey(e => e.Id);
        }

    }
}
