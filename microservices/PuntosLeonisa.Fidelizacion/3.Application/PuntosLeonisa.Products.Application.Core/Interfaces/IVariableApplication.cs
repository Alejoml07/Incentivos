using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Variables;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Fidelizacion.Application.Core.Interfaces
{
    public interface IVariableApplication
    {
        Task<GenericResponse<bool>> AddVariable(VariableDto value);  
        Task<GenericResponse<bool>> DeleteVariableById(string id);
        Task<GenericResponse<VariableDto>> GetVariableById(string id);
        Task<GenericResponse<VariableDto>> GetVariable();


    }
}
