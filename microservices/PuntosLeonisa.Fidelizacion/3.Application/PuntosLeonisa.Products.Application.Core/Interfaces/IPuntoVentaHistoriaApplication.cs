using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Application.Core.Interfaces
{
    public interface IPuntoVentaHistoriaApplication
    {
        Task<GenericResponse<IEnumerable<PuntoVentaHistoriaDto[]>>> AddPuntoVentaHistoria(PuntoVentaHistoriaDto[] data);
    }
}
