using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta
{
    public class LiquidacionPuntos
    {
        public Fecha? Fecha { get; set; }
        public PuntoVentaVarDto? Registro { get; set; }
    }

    public class Fecha
    {
        public string? Mes { get; set; }
        public string? Anho { get; set; }
    }

    
}
