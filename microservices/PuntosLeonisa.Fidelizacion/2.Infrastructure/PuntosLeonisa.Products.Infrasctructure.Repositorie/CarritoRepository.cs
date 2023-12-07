using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class CarritoRepository : Repository<CarritoDto>, ICarritoRepository
    {
        internal FidelizacionContext _context;
        public CarritoRepository(FidelizacionContext context) : base(context)
        {
            _context = context;
        }
    }
}
