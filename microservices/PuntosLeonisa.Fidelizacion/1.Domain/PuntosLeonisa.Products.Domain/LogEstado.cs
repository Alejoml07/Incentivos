using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain
{
    public class LogEstado
    {
        public string? Id { get; set; }
        public string? Estado { get; set; }
        public string? Usuario { get; set; }
        public DateTime? FechaCambioEstado { get; set; }
        public string? EAN { get; set; }
        public string? NroGuia { get; set; }
        public string? Transportadora { get; set; }
        public int? NroPedido { get; set; }
    }
}
