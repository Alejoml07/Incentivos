using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Products.Domain.Service.DTO.Productos
{
    public class ProductoPreciosDto
    {
        public string? EAN { get; set; }
        public double? Precio { get; set; }
        public double? PrecioOferta { get; set; }
        public float? Puntos { get; set; }

        public float? PuntosSinDescuento { get; set; }
    }
}
