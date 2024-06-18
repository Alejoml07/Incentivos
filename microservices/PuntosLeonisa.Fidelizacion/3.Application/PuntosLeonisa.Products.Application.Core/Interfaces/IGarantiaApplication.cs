using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Garantias;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Fidelizacion.Application.Core.Interfaces
{
    public interface IGarantiaApplication
    {
        Task<GenericResponse<GarantiaDto>> AddGarantia(GarantiaDto data);
    }
}
