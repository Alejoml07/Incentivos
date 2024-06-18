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
    public class GarantiaRepository : Repository<Garantia>, IGarantiaRepository
    {
        private readonly FidelizacionContext context;
        public GarantiaRepository(FidelizacionContext context) : base(context)
        {
            this.context = context;
        }

        public int GetNroGarantia()
        {
            return this.context.Set<Garantia>().Count();
        }
    }
}
