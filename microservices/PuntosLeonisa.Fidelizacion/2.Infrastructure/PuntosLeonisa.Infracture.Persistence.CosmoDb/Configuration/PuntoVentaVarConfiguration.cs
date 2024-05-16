using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using PuntosLeonisa.Fidelizacion.Domain.Model;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class PuntoVentaVarConfiguration : IEntityTypeConfiguration<PuntoVentaVar>
    {
        public void Configure(EntityTypeBuilder<PuntoVentaVar> builder)
        {
            builder.ToContainer("PuntoVentaVar")
                .HasPartitionKey(e => e.Id);
        }

    }
}
