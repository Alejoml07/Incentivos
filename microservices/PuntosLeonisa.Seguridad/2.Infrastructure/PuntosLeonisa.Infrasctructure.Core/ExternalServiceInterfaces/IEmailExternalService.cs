using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Infrasctructure.Core.ExternalServiceInterfaces
{
    public interface IEmailExternalService
    {
        Task<GenericResponse<bool>> SendMailForResetPasswordByUser(UsuarioDto data,string urlReset);
    }
}
