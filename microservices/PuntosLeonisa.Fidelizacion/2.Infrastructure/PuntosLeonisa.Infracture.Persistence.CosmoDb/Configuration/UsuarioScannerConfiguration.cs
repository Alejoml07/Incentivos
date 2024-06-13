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
    public class UsuarioScannerConfiguration : IEntityTypeConfiguration<UsuarioScanner>
    {
        public void Configure(EntityTypeBuilder<UsuarioScanner> builder)
        {
            builder.ToContainer("UsuarioScanner")
                .HasPartitionKey(e => e.Id).HasKey(p => p.Id);
        }
    }
}
