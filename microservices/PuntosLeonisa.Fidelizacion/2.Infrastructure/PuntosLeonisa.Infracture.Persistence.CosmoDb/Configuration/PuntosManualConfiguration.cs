using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Products.Domain.Model;

namespace PuntosLeonisa.Infrasctructure.Core.Configuration
{
    public class PuntosManualConfiguration : IEntityTypeConfiguration<PuntosManual>
    {
        public void Configure(EntityTypeBuilder<PuntosManual> builder)
        {
            builder.ToContainer("PuntosManuales")
                .HasPartitionKey(e => e.Id);
        }

    }
}
