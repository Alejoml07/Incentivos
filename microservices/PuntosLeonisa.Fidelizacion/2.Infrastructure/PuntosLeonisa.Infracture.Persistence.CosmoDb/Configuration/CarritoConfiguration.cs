using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class CarritoConfiguration : IEntityTypeConfiguration<CarritoDto>
    {
        public void Configure(EntityTypeBuilder<CarritoDto> builder)
        {
            builder.ToContainer("Carrito")
                .HasPartitionKey(e => e.Id);
        }
    }
}
