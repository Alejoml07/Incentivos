using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.FidelizacionPuntos
{
    public class LiquidacionPuntosDto
    {

        public string Cedula { get; set; }
        public string Id_User { get; set; }

        public int Mes { get; set; }
        public int Anho { get; set; }

        public int? Porcentaje { get; set; }

        public int? Id_Variable { get; set; }

        public int? Puntos { get; set; }

        public int? Publico { get; set; }
    }
}
