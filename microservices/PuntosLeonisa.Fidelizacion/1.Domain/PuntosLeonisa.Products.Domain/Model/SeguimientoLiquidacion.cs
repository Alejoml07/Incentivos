using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Model
{
    public class SeguimientoLiquidacion
    {
        public string? Id { get; set; }
        public string? Cedula { get; set; }
        public string? NombrePtoVenta { get; set; }
        public string? NombreVariable { get; set; }
        public string? Mes { get; set; }
        public string? Anio { get; set; }
        public string? PtoVenta { get; set; }
        public double? Cumplimiento { get; set; }
        public double? Puntos { get; set; }
        public string? IdVariable { get; set; }
        public string? Porcentaje { get; set; }
    }
}
