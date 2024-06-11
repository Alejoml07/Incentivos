using PuntosLeonisa.Fidelizacion.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion
{
    public class OrdenDto
    {
        public string? Id { get; set; }

        public int? NroPedido { get; set; }

        public UsuarioDtoLite? Usuario { get; set; }

        public IEnumerable<ProductoCarritoLite>? ProductosCarrito { get; set; }

        public UsuarioEnvio? Envio { get; set; }

        public EstadoOrden? Estado { get; set; }
        public DateTime? FechaRedencion { get; set; }

        public string? ValorMovimiento { get; set; }

    }

    public class UsuarioEnvio
    {
        public string? Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? Departamento { get; set; }
        public string? Celular { get; set; }
        public string? CelularSecundario { get; set; }
        public string? Email { get; set; }
        public string? TipoDocumento { get; set; }
        public string? Documento { get; set; }
        public string? TipoEnvio { get; set; }
        public string? Observaciones { get; set; }
        public string? Estado { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Pais { get; set; }

    }   

    public class ProductoCarritoLite
    {
        public string Id { get; set; }

        public string? Referencia { get; set; }

        public string? Nombre { get; set; }

        public EstadoOrdenItem? Estado { get; set; }

        public int? Cantidad { get; set; }

        public string? NroGuia { get; set; }

        public string? Transportadora { get; set; }

        public string? Descripcion { get; set; }

        public float? Puntos { get; set; }

        public string? UrlImagen1 { get; set; }

        public string? Proveedor { get; set; }

        public ProveedorLite? ProveedorLites { get; set; }

        public string? Email { get; set; }

        public string? TipoPremio { get; set; }

        public string? EAN { get; set; }

        public string? Marca { get; set; }

        public int? Quantity { get; set; }

        public string? Precio { get; set; }

    }

    public class  UsuarioDtoLite
    {
        public string? Cedula { get; set; }

        public string? Celular { get; set; }

        public string? Nombres { get; set; }

        public string? Apellidos { get; set; }

        public string? Email { get; set; }

        public string? TipoUsuario { get; set; }
    }
}
