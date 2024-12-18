using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuntosLeonisa.Fidelizacion.Domain;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class LogEstadoConfiguration : IEntityTypeConfiguration<LogEstado>
    {
        public void Configure(EntityTypeBuilder<LogEstado> builder)
        {
            builder.ToContainer("LogEstado")
                .HasPartitionKey(e => e.Id);
        }
    }
}
