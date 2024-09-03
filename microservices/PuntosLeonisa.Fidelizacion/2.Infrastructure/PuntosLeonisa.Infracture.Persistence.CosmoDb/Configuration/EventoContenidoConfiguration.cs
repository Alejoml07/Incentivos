using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class EventoContenidoConfiguration: IEntityTypeConfiguration<EventoContenido>
    {
        public void Configure(EntityTypeBuilder<EventoContenido> builder)
        {
            builder.ToContainer("EventoContenido")
                .HasPartitionKey(e => e.Id);
        }
    }
}
