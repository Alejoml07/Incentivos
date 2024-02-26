using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Fidelizacion.Application.Core.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.WishList;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Products.Application.Core.Interfaces;

namespace PuntosLeonisa.Seguridad.Application.Core;

public interface IFidelizacionApplication : IApplicationCore<PuntosManualDto>, IUsuarioInfoPuntosApplication
{
    Task<GenericResponse<WishListDto>> WishListAdd(WishListDto wishList);
    Task<bool> WishListDeleteById(string id);
    Task<GenericResponse<IEnumerable<WishListDto>>> WishListGetByUser(string id);
    Task<GenericResponse<Carrito>> CarritoAdd(Carrito carrito);
    Task<bool> CarritoDeleteById(string id);
    Task<GenericResponse<IEnumerable<Carrito>>> CarritoGetByUser(string id);
    Task<GenericResponse<bool>> ValidateCodeRedencion(SmsDto data);
    Task<GenericResponse<bool>> AddExtracto(Extractos data);
    Task<GenericResponse<IEnumerable<Extractos>>> GetExtractos();
    Task<GenericResponse<IEnumerable<bool>>> AddExtractos(Extractos[] data);
    Task<GenericResponse<IEnumerable<Extractos>>> GetExtractosByUsuario(string cedula); 
    Task<GenericResponse<IEnumerable<UsuarioRedencion>>> GetReporteRedencion(ReporteDto data);
    Task<GenericResponse<IEnumerable<Extractos>>> GetExtractosByUserAndDate(ReporteDto data);
    Task<GenericResponse<IEnumerable<Extractos>>> UpdateMesYAño(ReporteDto data);
    Task<GenericResponse<IEnumerable<Extractos>>> UpdateUser();
}

