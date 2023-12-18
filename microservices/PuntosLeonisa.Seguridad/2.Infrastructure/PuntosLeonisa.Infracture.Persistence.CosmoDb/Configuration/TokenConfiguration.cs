using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class TokenConfiguration : IEntityTypeConfiguration<TokenDto>
    {
        public void Configure(EntityTypeBuilder<TokenDto> builder)
        {
            builder.ToContainer("TokenContrasena")
                .HasPartitionKey(e => e.Id);
        }

    }
}
