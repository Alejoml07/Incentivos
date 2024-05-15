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
        public PuntoDeVentaRepository(FidelizacionContext context) : base(context)
        {
        }
    }
}
