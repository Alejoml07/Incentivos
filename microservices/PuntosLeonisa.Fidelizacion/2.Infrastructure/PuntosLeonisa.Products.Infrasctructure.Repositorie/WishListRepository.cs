using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.WishList;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;

namespace PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie
{
    public class WishListRepository : Repository<WishListDto>, IWishListRepository
    {
        internal FidelizacionContext _context;
        public WishListRepository(FidelizacionContext context) : base(context)
        {
            _context = context;
        }


    }
}
