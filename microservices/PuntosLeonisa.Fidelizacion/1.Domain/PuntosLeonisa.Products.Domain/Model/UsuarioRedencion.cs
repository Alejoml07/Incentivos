using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;
using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Model
{
    public class UsuarioRedencion
    {
        public string Id { get; set; }
        public Usuario Usuario { get; set; }

        public UsuarioInfoPuntos? InfoPuntos { get; set; }

        public IEnumerable<ProductoRefence> ProductosCarrito { get; set; }

        public UsuarioEnvio Envio { get; set; }


        public int? PuntosRedimidos
        {
            get { return this.ProductosCarrito.Sum(p => Convert.ToInt32(p.Puntos) * p.Quantity); }
        }


        public DateTime FechaRedencion { get; set; }

    }

    public class UsuarioEnvio
    {
        public string Id { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Direccion { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public string Ciudad { get; set; }
        public string Departamento { get; set; }
        public string Observaciones { get; set; }

    }
}
