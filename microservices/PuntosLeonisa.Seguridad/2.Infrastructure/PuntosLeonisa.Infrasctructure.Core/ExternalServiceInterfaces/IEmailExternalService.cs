using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Infrasctructure.Core.ExternalServiceInterfaces
{
    public interface IEmailExternalService
    {
        Task<GenericResponse<bool>> UserSendEmailWithMessage(UsuarioRedencion data);
    }
}
