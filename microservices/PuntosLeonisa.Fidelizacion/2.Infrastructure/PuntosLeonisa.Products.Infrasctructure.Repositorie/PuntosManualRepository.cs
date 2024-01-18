using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Model;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie;
public class MovimientoPuntosRepository : Repository<MovimientoPuntos>, IMovimientoPuntosRepository
{
    public MovimientoPuntosRepository(FidelizacionContext context) : base(context)
    {
    }
}

