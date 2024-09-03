using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.FidelizacionPuntos;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.MovimientoPuntos;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using System.Globalization;

namespace PuntosLeonisa.Fidelizacion.Application.Core.Interfaces
{
    public interface IUsuarioInfoPuntosApplication
    {

        Task<GenericResponse<UsuarioInfoPuntos>> GetUsuarioInfoPuntosById(string id);
        Task<GenericResponse<UsuarioInfoPuntos>> AddUsuarioInfoPuntos(UsuarioInfoPuntos value);
        Task<GenericResponse<UsuarioInfoPuntos[]>> AddUsuarioInfoPuntosRange(UsuarioInfoPuntos[] value);
        Task<GenericResponse<UsuarioInfoPuntos>> UpdateUsuarioInfoPuntos(UsuarioInfoPuntos value);
        Task<GenericResponse<UsuarioInfoPuntos>> DeleteUsuarioInfoPuntos(UsuarioInfoPuntos value);
        Task<GenericResponse<UsuarioInfoPuntos>> DeleteUsuarioInfoPuntosById(string id);
        Task<GenericResponse<IEnumerable<UsuarioInfoPuntos>>> GetUsuarioInfoPuntosAll();
        Task<GenericResponse<bool>> RedencionPuntos(UsuarioRedencion data);
        Task<GenericResponse<IEnumerable<OrdenDto>>> GetUsuariosRedencionPuntos();
        Task<GenericResponse<OrdenDto>> GetUsuariosRedencionPuntosById(string id);
        Task<GenericResponse<SmsDto>> SaveCodeAndSendSms(SmsDto data);
        Task<GenericResponse<bool>> CreateRedencion(UsuarioRedencion data);
        Task<GenericResponse<IEnumerable<OrdenDto>>> GetUsuariosRedencionPuntosByProveedor(string proveedor);
        Task<GenericResponse<IEnumerable<UsuarioRedencion>>> GetUsuariosByTipoUsuarioAndProveedor(TipoUsuarioDto[] data);
        Task<GenericResponse<IEnumerable<OrdenDto>>> GetUsuariosRedencionPuntosByEmail(string email);
        Task<GenericResponse<AddNroGuiaYTransportadora>> AddNroGuiaYTransportadora(AddNroGuiaYTransportadora data);
        Task<GenericResponse<int>> DevolucionPuntosYCancelarEstado(DevolucionPuntosDto data);
        Task<GenericResponse<bool>> GuardarLiquidacionPuntos(IEnumerable<LiquidacionPuntosDto> data);
        Task<GenericResponse<bool>> CambiarEstadoYLiquidarPuntos(string email);
        Task<GenericResponse<bool>> UpdateCorreoInfoPuntos(UpdateInfoDto data);
        Task<GenericResponse<bool>> ActualizarYCrearInfoPuntos();


    }
}
