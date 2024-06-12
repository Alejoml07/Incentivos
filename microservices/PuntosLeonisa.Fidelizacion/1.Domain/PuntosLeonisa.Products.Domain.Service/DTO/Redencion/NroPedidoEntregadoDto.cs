using PuntosLeonisa.Fidelizacion.Domain.Model;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion
{
    public class NroPedidoEntregadoDto
    {
        public int NroPedido { get; set; }
        public ProductoRefence? Producto { get; set; }
    }
}
