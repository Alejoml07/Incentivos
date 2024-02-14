using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Products.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuntosLeonisa.Products.Domain.Service.DTO.GestorDeContenido;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class GestorDeContenidoConfiguration : IEntityTypeConfiguration<GestorDeContenidoDto>
    {
        public void Configure(EntityTypeBuilder<GestorDeContenidoDto> builder)
        {
            builder.ToContainer("Banner")
                .HasPartitionKey(e => e.Id).HasKey(e => e.Id);
        }
    }
}
