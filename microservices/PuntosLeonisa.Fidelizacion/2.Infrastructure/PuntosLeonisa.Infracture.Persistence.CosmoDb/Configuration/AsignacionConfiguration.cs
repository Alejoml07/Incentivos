using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuntosLeonisa.Fidelizacion.Domain.Model;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class AsignacionConfiguration : IEntityTypeConfiguration<Asignacion>
    {
        public void Configure(EntityTypeBuilder<Asignacion> builder)
        {
            builder.ToContainer("Asignacion")
                .HasPartitionKey(e => e.Id);
        }
    }
}
