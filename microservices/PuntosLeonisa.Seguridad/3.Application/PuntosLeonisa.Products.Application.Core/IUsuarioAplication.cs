using PuntosLeonisa.Products.Application.Core.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Seguridad.Application.Core;

public interface IUsuarioApplication : IApplicationCore<UsuarioDto>
{
    Task<GenericResponse<UsuarioResponseLiteDto>> Authentication(LoginDto login);

    Task<GenericResponse<bool>> CambiarPwd(CambioPwdDto cambioContraseñaDto);
}

