using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.DTO;
using PuntosLeonisa.Products.Domain.Model;

namespace PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces
{
    public interface IUsuarioExternalService
    {
        Task<GenericResponse<Usuario>> GetUserLiteByCedula(string cedula);
        Task<GenericResponse<Usuario>> GetUserByEmail(string email);
        Task<bool> SendSmsWithCode(SmsDto data);
        Task<GenericResponse<bool>> SendSmsWithMessage(Usuario data, string message);
        Task<GenericResponse<bool>> UserSendEmailWithMessage(UsuarioRedencion data);
        Task<GenericResponse<bool>> UserSendEmailWithMessageAndState(UsuarioRedencion data);
        Task<GenericResponse<bool>> SendMailGeneric(EmailDTO emailData);       
        Task<GenericResponse<bool>> SendMailGarantia(Garantia data);
        Task<GenericResponse<bool>> SendMailGarantiaEnviada(Garantia datan, string CorreoProveedor);
        Task<GenericResponse<bool>> CambiarEstado(string email);
        Task<GenericResponse<IEnumerable<Usuario>>> GetUsuarios();
        Task<IEnumerable<Porcentajes>> GetUsuarioTPA(Fecha data, string token);
        Task<UsuarioTpa> ValidarUsuario(ValidarUsuarioDto data);
        Task<GenericResponse<Usuario>> AddUsuarioLiquidacion(Usuario data);
    }
}
