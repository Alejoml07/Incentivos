using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Seguridad.Infrasctructure.Repositorie
{
    public class PuntoDeVentaRepository : Repository<PuntoDeVenta>, IPuntoDeVentaRepository
    {
        internal SeguridadContext _context;
        public PuntoDeVentaRepository(SeguridadContext context) : base(context)
        {
            _context = context;
        }
    }
}
