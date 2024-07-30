using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using System.Data.Common;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class SeguimientoLiquidacionRepository : Repository<SeguimientoLiquidacion>, ISeguimientoLiquidacionRepository
    {
        private readonly FidelizacionContext context;
        public SeguimientoLiquidacionRepository(FidelizacionContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<SeguimientoLiquidacion>> GetSeguimientoLiquidacion(Fechas data)
        {
            // Traer todos los seguimientos
            var seguimientos = await context.Set<SeguimientoLiquidacion>()
                                             .Where(x => x.Mes == data.Mes && x.Anio == data.Anio)
                                             .ToListAsync();
            return seguimientos.Where(x => x.Puntos != 0);
        }
    }
}
