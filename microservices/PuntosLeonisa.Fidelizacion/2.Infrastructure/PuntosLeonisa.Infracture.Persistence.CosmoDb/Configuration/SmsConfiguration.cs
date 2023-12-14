using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class SmsConfiguration : IEntityTypeConfiguration<SmsDto>
    {
        public void Configure(EntityTypeBuilder<SmsDto> builder)
        {
            builder.ToContainer("Token")
                .HasPartitionKey(e => e.Id);
        }

    }
}
