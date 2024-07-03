using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Model.Carrito;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.FidelizacionPuntos;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class PuntoVentaVarRepository : Repository<PuntoVentaVar>, IPuntoVentaVarRepository
    {
        private readonly FidelizacionContext context;
        public PuntoVentaVarRepository(FidelizacionContext context) : base(context)
        {
            this.context = context;
        }

        public Task<PuntoVentaVar> GetPuntoVentaVar(PuntoVentaVarDto data)
        {
            var response = context.Set<PuntoVentaVar>().Where(x => x.IdVariable == data.IdPuntoVenta && x.IdVariable == data.IdVariable && x.Mes == data.Mes && x.Anio == data.Anio).FirstOrDefault();
            return Task.FromResult(response);
        }

        public async Task<IEnumerable<PuntoVentaVar>> GetPuntoVentaVarByMesAndAnio(PuntoVentaVar data)
        {
            var response = await context.Set<PuntoVentaVar>().Where(x => x.Mes == data.Mes && x.Anio == data.Anio).ToListAsync();
            return response;
        }
    }
}
