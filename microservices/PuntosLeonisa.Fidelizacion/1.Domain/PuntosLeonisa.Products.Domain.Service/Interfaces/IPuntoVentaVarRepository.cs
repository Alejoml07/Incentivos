using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.FidelizacionPuntos;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces
{
    public interface IPuntoVentaVarRepository : IRepository<PuntoVentaVar>
    {
        Task<IEnumerable<PuntoVentaVar>> DeletePuntoVentaVarByMesAndAnio(PuntoVentaVar data);
        Task<PuntoVentaVar> GetPuntoVentaVar(PuntoVentaVarDto data);
        Task<IEnumerable<PuntoVentaVar>> GetPuntosByCodigoUsuario(PuntoVentaVar data);
        Task<PuntoVentaVar> GetConsultaPresupuesto(PuntoVentaVar data);
    }
}
