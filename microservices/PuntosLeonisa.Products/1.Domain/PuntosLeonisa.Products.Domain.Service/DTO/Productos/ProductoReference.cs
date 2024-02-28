using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Products.Domain.Model
{
    public class ProductoRefence
    {

        public ProductoRefence()
        {
            this.Estado = this.GetEstadoOrdenItem();
        }

        public string Id { get; set; }

        public string? Referencia { get; set; }

        public string? Nombre { get; set; }

        public string? Video { get; set; }

        public int? Cantidad { get; set; }

        public string? NroGuia { get; set; }

        public string? Transportadora { get; set; }

        public string? Caracteristicas { get; set; }

        public string? Descripcion { get; set; }

        public float? Puntos { get; set; }

        public float? PuntosSinDescuento { get; set; }

        public float? TiempoEntrega { get; set; }

        public EstadoOrdenItem? Estado
        {
            get;

            set;

        }

        public DateTime? FechaCreacion { get; set; }

        public string? ImagenPrincipal { get; set; }

        public string? UrlImagen1 { get; set; }

        public string? UrlImagen2 { get; set; }

        public string? UrlImagen3 { get; set; }

        public string? UrlImagen4 { get; set; }

        public string? UrlImagen5 { get; set; }

        public string? Proveedor { get; set; }

        public string? Correo { get; set; }

        public string? TipoPremio { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public string? EAN { get; set; }

        public string? Marca { get; set; }

        public string? Color { get; set; }

        public string? Talla { get; set; }

        public float? Rating { get; set; }

        public double? PrecioOferta { get; set; }

        public double? Precio { get; set; }

        public string? Usuario { get; set; }

        public string? UsuarioModificacion { get; set; }

        public string? CategoriaNombre { get; set; }

        public string? SubCategoriaNombre { get; set; }

        public string? PalabrasClaves { get; set; }

        public int? TipoIva { get; set; }

        public string? Alto { get; set; }

        public string? Ancho { get; set; }

        public string? Largo { get; set; }

        public string? Peso { get; set; }

        public DateTime? FechaSincronizacion { get; set; }

        public ProveedorLite? ProveedorLite { get; set; }

        public string? Invima { get; set; }

        public string? Genero { get; set; }

        public string? Tamaño { get; set; }

        public int? Quantity { get; set; }

        public EstadoOrdenItem GetEstadoOrdenItem()
        {

            if (Estado == EstadoOrdenItem.Cancelado)
            {
                return EstadoOrdenItem.Cancelado;
            }
            if (Estado == EstadoOrdenItem.Entregado)
            {
                return EstadoOrdenItem.Entregado;
            }
            if (NroGuia != null && Transportadora != null)
            {
                return EstadoOrdenItem.Enviado;
            }
            return EstadoOrdenItem.Pendiente;

        }


    }
}
        public enum EstadoOrdenItem
        {
            Pendiente,
            Enviado,
            EnTransito,
            Cancelado,
            Entregado
        }
    

