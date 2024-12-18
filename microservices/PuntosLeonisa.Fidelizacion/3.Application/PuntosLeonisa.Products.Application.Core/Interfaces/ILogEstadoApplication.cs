using PuntosLeonisa.Fidelizacion.Domain;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Application.Core.Interfaces
{
    public interface ILogEstadoApplication
    {
        Task<GenericResponse<bool>> AddLogEstado(LogEstado data);
        Task<GenericResponse<bool>> AddLogsEstado(LogEstado[] data);
    }
}
