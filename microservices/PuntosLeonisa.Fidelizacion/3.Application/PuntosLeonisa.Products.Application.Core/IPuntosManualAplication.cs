using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Products.Application.Core.Interfaces;

namespace PuntosLeonisa.Seguridad.Application.Core;

public interface IPuntosManualApplication : IApplicationCore<PuntosManualDto>
{
    Task<IEnumerable<WishListDto>> WishList(WishListDto wishList);
}

