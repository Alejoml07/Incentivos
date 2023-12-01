using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Interfaces
{
    public interface IWishListRepository : IRepository<WishListDto>
    {
        Task<IEnumerable<WishListDto>> WishList(WishListDto wishList);
    }
}
