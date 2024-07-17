using PuntosLeonisa.Products.Application.Core.Interfaces;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Seguridad.Application.Core;

public interface IUsuarioApplication : IApplicationCore<UsuarioDto>
{
    Task<GenericResponse<UsuarioResponseLiteDto>> Authentication(LoginDto login);
    Task<GenericResponse<bool>> CambiarPwd(CambioPwdDto cambioContraseñaDto);
    Task<GenericResponse<UsuarioResponseLiteDto>> GetByEmail(string email);
    Task<GenericResponse<bool>> RecuperarPassword(UsuarioDto data);
    Task<GenericResponse<bool>> CambioRecuperarPwd(CambioRecuperarPwdDto data);
    Task<GenericResponse<UsuarioDto>> ValidarTokenCambiarContrasena(TokenDto token);
    Task<GenericResponse<bool>> ValidarCorreo(string email);
    Task<GenericResponse<bool>> CambiarEstado(string email);
    Task<GenericResponse<IEnumerable<UsuarioBasicDto>>> GetUsuarioBasic();
    Task<GenericResponse<bool>> ResetearTodasLasContrasenas();
    Task<GenericResponse<bool>> UpdateEmailSinEspacios();
    Task<GenericResponse<IEnumerable<Usuario>>> GetUsuariosByTipoUsuario(TiposUsuarioDto[] data);
}

