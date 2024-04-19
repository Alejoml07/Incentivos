using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;

namespace PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces
{
    public interface IOrdenOPExternalService
    {
        Task<bool> EnviarOrdenOP(OrdenOP ordenOP);
        Task<ResultNroPedidoOp> GetNroOrdenOP(NroPedidoOP nroOrden);
        
    }
}
