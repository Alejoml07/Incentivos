using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class PuntoVentaVarConfiguration : IEntityTypeConfiguration<PuntoVentaVarDto>
    {
        public void Configure(EntityTypeBuilder<PuntoVentaVarDto> builder)
        {
            builder.ToContainer("PuntoVentaVar")
                .HasPartitionKey(e => e.Id);
        }

    }
}
