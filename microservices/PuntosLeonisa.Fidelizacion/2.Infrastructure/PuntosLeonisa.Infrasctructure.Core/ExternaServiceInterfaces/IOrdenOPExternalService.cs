using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces
{
    public interface IOrdenOPExternalService
    {
        Task<bool> EnviarOrdenOP(OrdenOP ordenOP);
        Task<int> GetNroOrdenOP(NroPedidoOP nroOrden);
        
    }
}
