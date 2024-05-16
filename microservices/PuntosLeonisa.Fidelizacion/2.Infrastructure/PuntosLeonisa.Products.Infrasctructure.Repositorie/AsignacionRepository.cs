using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class AsignacionRepository : Repository<Asignacion>, IAsignacionRepository
    {
        private readonly FidelizacionContext context;
        public AsignacionRepository(FidelizacionContext context) : base(context)
        {
            this.context = context;
        }
    }
}
