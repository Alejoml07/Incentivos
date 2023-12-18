using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class UsuarioRedencionRepository : Repository<UsuarioRedencion>, IUsuarioRedencionRepository
    {
        public UsuarioRedencionRepository(FidelizacionContext context) : base(context)
        {
        }
    }
}
