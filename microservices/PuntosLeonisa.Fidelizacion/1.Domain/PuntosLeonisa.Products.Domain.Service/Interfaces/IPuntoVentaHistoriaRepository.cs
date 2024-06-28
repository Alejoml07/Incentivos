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
    public interface IPuntoVentaHistoriaRepository : IRepository<PuntoVentaHistoria>
    {
        Task<IEnumerable<PuntoVentaHistoria>> GetPuntoVentaHistoriaByMesAndAnio(LiquidacionPuntos data);
        Task<IEnumerable<PuntoVentaHistoria>> GetPuntoVentaHistoriaById(PuntoVentaHistoria data);
    }
}
