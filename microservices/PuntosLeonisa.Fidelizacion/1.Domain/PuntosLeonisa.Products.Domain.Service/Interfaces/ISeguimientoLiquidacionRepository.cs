using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces
{
    public interface ISeguimientoLiquidacionRepository :IRepository<SeguimientoLiquidacion>
    {
        Task<IEnumerable<SeguimientoLiquidacion>> GetSeguimientoLiquidacion(Fechas data);
    }
}
