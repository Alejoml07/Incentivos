using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class UsuarioRedencionRepository : Repository<UsuarioRedencion>, IUsuarioRedencionRepository
    {
        private readonly FidelizacionContext context;

        public UsuarioRedencionRepository(FidelizacionContext context) : base(context)
        {
            this.context = context;
        }

        public int GetNroPedido()
        {
            return this.context.Set<UsuarioRedencion>().Count();
        }
    }
}
