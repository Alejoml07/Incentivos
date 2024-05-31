using PuntosLeonisa.Fidelizacion.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion
{
    public class MetricasGeneralDto
    {
        public int? ContadorPendiente { get; set; }
        public UsuarioRedencion? Redencion { get; set; }
    }
}
