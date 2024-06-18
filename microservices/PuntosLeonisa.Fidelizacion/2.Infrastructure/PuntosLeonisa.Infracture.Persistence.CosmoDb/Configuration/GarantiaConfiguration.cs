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
    public class GarantiaConfiguration : IEntityTypeConfiguration<Garantia>
    {
        public void Configure(EntityTypeBuilder<Garantia> builder)
        {
            builder.ToContainer("Garantia")
                .HasPartitionKey(e => e.Id);
        }
    }
}
