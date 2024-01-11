using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntosLeonisa.Fidelizacion.Domain.Model;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class FidelizacionPuntosConfiguration : IEntityTypeConfiguration<FidelizacionPuntos>
    {
        public void Configure(EntityTypeBuilder<FidelizacionPuntos> builder)
        {
            builder.ToContainer("FidelizacionPuntos")
                .HasPartitionKey(e => e.Id);
        }
    }
}
