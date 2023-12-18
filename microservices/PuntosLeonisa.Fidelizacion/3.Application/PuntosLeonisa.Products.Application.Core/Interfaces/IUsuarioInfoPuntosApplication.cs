using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;

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
        Task<GenericResponse<SmsDto>> SaveCodeAndSendSms(SmsDto data);
        Task<GenericResponse<bool>> CreateRedencion(UsuarioRedencion data);
    }
}
