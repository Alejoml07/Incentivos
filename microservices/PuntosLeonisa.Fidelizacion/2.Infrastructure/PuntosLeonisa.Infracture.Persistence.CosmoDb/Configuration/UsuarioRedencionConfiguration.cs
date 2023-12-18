using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.WishList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class UsuarioRedencionConfiguration : IEntityTypeConfiguration<UsuarioRedencion>
    {
        
        public void Configure(EntityTypeBuilder<UsuarioRedencion> builder)
        {
            builder.ToContainer("UsuarioRedencion")
                .HasPartitionKey(e => e.Id);
        }
    }
}
