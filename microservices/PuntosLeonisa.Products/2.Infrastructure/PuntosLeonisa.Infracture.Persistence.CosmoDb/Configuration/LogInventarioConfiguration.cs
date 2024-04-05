using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Products.Domain.Service.DTO.Productos;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class LogInventarioConfiguration : IEntityTypeConfiguration<LogInventarioDto>
    {
        public void Configure(EntityTypeBuilder<LogInventarioDto> builder)
        {
            builder.ToContainer("LogInventario")
                .HasPartitionKey(e => e.Id).HasKey(e => e.Id);
        }
    }
}
