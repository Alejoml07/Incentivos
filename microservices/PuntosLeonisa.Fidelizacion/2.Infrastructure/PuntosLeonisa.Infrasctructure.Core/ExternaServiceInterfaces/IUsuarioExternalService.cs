using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces
{
    public interface IUsuarioExternalService
    {
        Task<GenericResponse<Usuario>> GetUserLiteByCedula(string cedula);
    }
}
