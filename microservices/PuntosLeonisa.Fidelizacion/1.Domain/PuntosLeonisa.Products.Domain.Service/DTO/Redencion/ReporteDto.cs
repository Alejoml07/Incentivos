using PuntosLeonisa.Fidelizacion.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion
{
    public class ReporteDto
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string? Mes { get; set; }
        public string? Anio { get; set; }
        public string? Cedula { get; set; }
        public int? ContadorPendiente { get; set; }
        public int? ContadorEnviado { get; set; }
        public int? ContadorCancelado { get; set; }
        public string? TipoUsuario { get; set; }
        public string? Proveedor { get; set; }
    }
}
