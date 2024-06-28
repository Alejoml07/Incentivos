using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Model.Carrito;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;

namespace PuntosLeonisa.Seguridad.Infrasctructure.Repositorie
{
    public class PuntoDeVentaRepository : Repository<PuntoDeVenta>, IPuntoDeVentaRepository
    {
        internal FidelizacionContext _context;
        public PuntoDeVentaRepository(FidelizacionContext context) : base(context)
        {
            _context = context;
        }

        public Task<PuntoDeVenta> GetPuntoDeVentaByCodigo(int codigo)
        {
            var response = _context.Set<PuntoDeVenta>().Where(x => x.Codigo == codigo).FirstOrDefault();
            return Task.FromResult(response);
        }
    }
}
