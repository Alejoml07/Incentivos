using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Products.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuntosLeonisa.Products.Domain.Service.DTO.Banners;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class BannerConfiguration : IEntityTypeConfiguration<Banner>
    {
        public void Configure(EntityTypeBuilder<Banner> builder)
        {
            builder.ToContainer("Banner")
                .HasPartitionKey(e => e.TipoUsuario).HasKey(e => e.TipoUsuario);
        }
    }
}
