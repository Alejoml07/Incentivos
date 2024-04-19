using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion
{
    public class NroPedidoOP
    {
        public NroPedidoOP()
        {
            this.operationType = "Pedido";
        }
        public string? operationType { get; set; }
    }
}
