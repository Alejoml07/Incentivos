﻿using Microsoft.EntityFrameworkCore;
using Polly;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class PuntoVentaVarRepository : Repository<PuntoVentaVar>, IPuntoVentaVarRepository
    {
        private readonly FidelizacionContext context;
        public PuntoVentaVarRepository(FidelizacionContext context) : base(context)
        {
            this.context = context;
        }

        public Task<PuntoVentaVar> GetConsultaPresupuesto(PuntoVentaVar data)
        {
            var response = context.Set<PuntoVentaVar>().Where(x => x.IdVariable == data.IdVariable && x.IdPuntoVenta == data.IdPuntoVenta && x.Mes == data.Mes && x.Anio == data.Anio).FirstOrDefault();
            return Task.FromResult(response);
        }

        public async Task<IEnumerable<PuntoVentaVar>> GetPuntosByCodigoUsuario(PuntoVentaVar data)
        {
            var response = await context.Set<PuntoVentaVar>().Where(x => x.CodigoPuntoVenta == data.CodigoPuntoVenta && x.Mes == data.Mes && x.Anio == data.Anio && x.Cumplimiento >= 99.5).ToListAsync();
            return response;
        }

        public Task<PuntoVentaVar> GetPuntoVentaVar(PuntoVentaVarDto data)
        {
            var response = context.Set<PuntoVentaVar>().Where(x => x.IdPuntoVenta == data.IdPuntoVenta && x.IdVariable == data.IdVariable && x.Mes == data.Mes && x.Anio == data.Anio).FirstOrDefault();
            return Task.FromResult(response);
        }

        public async Task<IEnumerable<PuntoVentaVar>> DeletePuntoVentaVarByMesAndAnio(PuntoVentaVar data)
        {
            var registros = await context.Set<PuntoVentaVar>()
                   .Where(x => x.Mes == data.Mes && x.Anio == data.Anio)
                   .ToListAsync();
            context.Set<PuntoVentaVar>().RemoveRange(registros);
            return registros;
        }
    }
}
