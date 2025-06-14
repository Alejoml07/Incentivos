﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PuntosLeonisa.Fidelizacion.Domain.Model;

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
