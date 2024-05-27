using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion
{
    public class MetricasDto
    {
        public int? ContadorPendiente { get; set; }
        public int? ContadorEnviado { get; set; }
        public int? ContadorCancelado { get; set; }
    }
}
