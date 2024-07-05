using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class SeguimientoLiquidacionRepository : Repository<SeguimientoLiquidacion>, ISeguimientoLiquidacionRepository
    {
        private readonly FidelizacionContext context;
        public SeguimientoLiquidacionRepository(FidelizacionContext context) : base(context)
        {
            this.context = context;
        }
    }
}
