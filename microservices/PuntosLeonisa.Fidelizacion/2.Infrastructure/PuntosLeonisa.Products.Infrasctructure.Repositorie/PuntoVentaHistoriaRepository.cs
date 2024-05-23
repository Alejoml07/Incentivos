using PuntosLeonisa.Fidelizacion.Domain;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class PuntoVentaHistoriaRepository : Repository<PuntoVentaHistoria>, IPuntoVentaHistoriaRepository
    {
        public PuntoVentaHistoriaRepository(FidelizacionContext context) : base(context)
        {
        }
    }
}
