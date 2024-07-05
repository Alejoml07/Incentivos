using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class SeguimientoLiquidacionConfiguration : IEntityTypeConfiguration<SeguimientoLiquidacion>
    {
        public void Configure(EntityTypeBuilder<SeguimientoLiquidacion> builder)
        {
            builder.ToContainer("SeguimientoLiquidacion")
                .HasPartitionKey(e => e.Id);
        }
    }
}
