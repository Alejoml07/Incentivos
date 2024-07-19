using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Garantias;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Fidelizacion.Application.Core.Interfaces
{
    public interface IGarantiaApplication
    {
        Task<GenericResponse<bool>> AddGarantia(Garantia data);
        Task<GenericResponse<IEnumerable<GarantiaDto>>> GetGarantiasByProveedorOrAll(TipoUsuarioDto[] data);
        Task<GenericResponse<bool>> CambiarEstadosGarantia(Garantia data);
        Task<GenericResponse<IEnumerable<GarantiaDto>>> GetGarantiasByUser(string email);
    }
}
