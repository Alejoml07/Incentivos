using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain
{
    public class PuntoVentaHistoria
    {
        public string? Id { get; set; }
        public string? IdVariable { get; set; }
        public string? IdPuntoVenta { get; set; }
        public string? IdPresupuesto { get; set; }
        public string? Mes { get; set; }
        public string? Ano { get; set; }
        public string? Base { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
