using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Products.Domain.Model;

namespace PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces
{
    public interface IUsuarioExternalService
    {
        Task<GenericResponse<Usuario>> GetUserLiteByCedula(string cedula);
    }
}
