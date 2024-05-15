using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Model.Carrito;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class PuntoVentaVarConfiguration : IEntityTypeConfiguration<PuntoVentaVarDto>
    {
        public void Configure(EntityTypeBuilder<PuntoVentaVarDto> builder)
        {
            builder.ToContainer("PuntoDeVenta")
                .HasPartitionKey(e => e.Id);
        }

    }
}
