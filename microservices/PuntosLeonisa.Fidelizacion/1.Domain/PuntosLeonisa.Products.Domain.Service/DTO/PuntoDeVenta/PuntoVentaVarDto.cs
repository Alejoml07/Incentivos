using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta
{
    public class PuntoVentaVarDto
    {
        public string? Id { get; set; }
        public string? IdPuntoVenta { get; set; }
        public string? IdVariable { get; set; }
        public string? CodigoPuntoVenta { get; set; }
        public string? NombrePtoVenta { get; set; }
        public string? NombreVariable { get; set; }
        public string? Mes { get; set; }
        public double? Base { get; set; }
        public string? Anio { get; set; }
        public string? Presupuesto { get; set; }
        public string? ValReal { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public double? Cumplimiento { get; set; }
    }
}
