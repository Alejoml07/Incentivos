using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntosLeonisa.Fidelizacion.Domain.Model;

namespace PuntosLeonisa.Infrasctructure.Core.Configuration
{
    public class MovimientoPuntosConfiguration : IEntityTypeConfiguration<MovimientoPuntos>
    {
        public void Configure(EntityTypeBuilder<MovimientoPuntos> builder)
        {
            builder.ToContainer("MovimientoPuntos")
                .HasPartitionKey(e => e.Id);
        }

    }
}
