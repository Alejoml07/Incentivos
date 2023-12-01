
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;
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

        public async Task<IEnumerable<WishListDto>> WishList(WishListDto wishList)
        {
            var data = await  _context.Set<WishListDto>().ToListAsync();
            return data;
        }
    }
}
