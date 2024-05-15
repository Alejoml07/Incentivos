using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Model
{
    public class Asignacion
    {
        public string? Id { get; set; }
        public string? IdPuntoVenta { get; set; }
        public string? IdCedula { get; set; }
        public float? Porcentaje { get; set; }
        public DateTime? Fecha { get; set; }
        public float? ValorReal { get; set; }
        public string? Mes { get; set; }
        public string? Anio { get; set; }
    }
}
