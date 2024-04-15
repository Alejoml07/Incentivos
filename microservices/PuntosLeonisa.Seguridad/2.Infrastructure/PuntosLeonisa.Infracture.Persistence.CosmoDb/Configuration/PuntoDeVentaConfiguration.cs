using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuntosLeonisa.Seguridad.Domain.Model;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class PuntoDeVentaConfiguration : IEntityTypeConfiguration<PuntoDeVenta>
    {
        public void Configure(EntityTypeBuilder<PuntoDeVenta> builder)
        {
            builder.ToContainer("PuntoDeVenta")
                .HasPartitionKey(e => e.Id);
        }

    }
}
