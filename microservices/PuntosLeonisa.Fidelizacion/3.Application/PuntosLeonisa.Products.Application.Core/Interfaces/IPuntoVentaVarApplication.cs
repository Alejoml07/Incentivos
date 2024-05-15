using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Application.Core.Interfaces
{
    public interface IPuntoVentaVarApplication
    {
        Task<GenericResponse<IEnumerable<PuntoVentaVarDto[]>>> AddPuntoVentaVar(PuntoVentaVarDto[] data);
    }
}
