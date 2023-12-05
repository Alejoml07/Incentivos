using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.WishList;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Products.Application.Core.Interfaces;

namespace PuntosLeonisa.Seguridad.Application.Core;

public interface IFidelizacionApplication : IApplicationCore<PuntosManualDto>
{
    Task<GenericResponse<WishListDto>> WishListAdd(WishListDto wishList);
    Task<bool> WishListDeleteById(string id);
    Task<GenericResponse<IEnumerable<WishListDto>>> WishListGetByUser(string id);
}

