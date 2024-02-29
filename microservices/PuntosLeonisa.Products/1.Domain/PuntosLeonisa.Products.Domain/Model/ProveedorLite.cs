using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Products.Domain.Model
{
    public class ProveedorLite
    {
        public string Id { get; set; }

        public string? Nit { get; set; }

        public string? Nombres { get; set; }

        public string? Email { get; set; }

        public double? Descuento { get; set; }
    }
}
