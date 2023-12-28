using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Products.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Infrasctructure.Core.ExternalServiceInterfaces
{
    public interface IProveedorExternalService
    {
        Task<GenericResponse<ProveedorLiteDto>> GetProveedorByNit(string nit);
    }
}
