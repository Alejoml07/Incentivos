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

        public IEnumerable<Carrito> ProductosCarrito { get; set; }


        public int PuntosRedimidos
        {
            get { return this.ProductosCarrito.Sum(p => Convert.ToInt32(p.Product.Puntos) * int.Parse(p.Cantidad)); }
        }


        public DateTime FechaRedencion { get; set; }

        public string Observaciones { get; set; }
    }
}
