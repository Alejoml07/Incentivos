﻿using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.WishList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;

namespace PuntosLeonisa.infrastructure.Persistence.Configuration
{
    public class CarritoConfiguration : IEntityTypeConfiguration<Carrito>
    {
        public void Configure(EntityTypeBuilder<Carrito> builder)
        {
            builder.ToContainer("Carrito")
                .HasPartitionKey(e => e.Id);
        }
    }
}
