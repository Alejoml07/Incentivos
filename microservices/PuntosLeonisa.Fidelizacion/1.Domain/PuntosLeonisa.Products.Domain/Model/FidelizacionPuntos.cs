using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Model
{
    public class FidelizacionPuntos
    {
        public string Id { get; set; }

        public Usuario Usuario { get; set; }

        public int Mes { get; set; }
        public int Anho { get; set; }

        public int? Porcentaje { get; set; }

        public int? Id_Variable { get; set; }

        public string PuntoVenta { get; set; }

        public int? Puntos { get; set; }
        public DateTime? FechaActualizacion { get; set; } = DateTime.Now;

        public DateTime? FechaCreacion { get; set; } = DateTime.Now;

        public int? Publico { get; set; }
        public string? Cedula { get; set; }
    }
}
