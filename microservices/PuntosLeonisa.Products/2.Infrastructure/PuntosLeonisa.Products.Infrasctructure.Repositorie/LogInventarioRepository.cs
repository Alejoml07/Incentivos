using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Products.Domain.Service.DTO.Productos;
using PuntosLeonisa.Products.Domain.Service.Interfaces;

namespace PuntosLeonisa.Products.Infrasctructure.Repositorie
{
    public class LogInventarioRepository : Repository<LogInventarioDto>, ILogInventarioRepository
    {
        private readonly ProductContext _context;
        public LogInventarioRepository(ProductContext context) : base(context)
        {
            _context = context;
        }
    }
}
