﻿using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class FidelizacionPuntosRepository : Repository<FidelizacionPuntos>, IFidelizacionPuntosRepository
    {
        private readonly FidelizacionContext context;
        public FidelizacionPuntosRepository(FidelizacionContext context) : base(context)
        {
            this.context = context;
        }
    }
}
