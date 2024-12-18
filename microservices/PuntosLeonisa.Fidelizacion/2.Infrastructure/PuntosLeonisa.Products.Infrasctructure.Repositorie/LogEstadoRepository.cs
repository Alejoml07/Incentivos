using PuntosLeonisa.Fidelizacion.Domain;
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
    public class LogEstadoRepository : Repository<LogEstado>, ILogEstadoRepository
    {
        private readonly FidelizacionContext context;
        public LogEstadoRepository(FidelizacionContext context) : base(context)
        {
            this.context = context;
        }
    }
}
