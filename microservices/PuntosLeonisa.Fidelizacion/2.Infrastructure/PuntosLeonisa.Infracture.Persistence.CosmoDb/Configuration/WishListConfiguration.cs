using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.WishList;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class WishListConfiguration : IEntityTypeConfiguration<WishListDto>
    {
        public void Configure(EntityTypeBuilder<WishListDto> builder)
        {
            builder.ToContainer("ListaDeseo")
                .HasPartitionKey(e => e.Id);
        }
    }
}
