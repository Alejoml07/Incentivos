using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Infrasctructure.Core.ExternalServiceInterfaces
{
    public interface IGetUsuarioExternalService
    {
        Task<UsuarioDto> GetUsuarioPorEmail(string cedula);
        Task<UsuarioDto> GetUsuarioPorCedula(string cedula);
        Task<UsuarioDto> GetUsuarioByCedula(string email);
        Task<GenericResponse<bool>> UpdateCorreoInfoPuntos(UpdateInfoDto data);
    }
}
