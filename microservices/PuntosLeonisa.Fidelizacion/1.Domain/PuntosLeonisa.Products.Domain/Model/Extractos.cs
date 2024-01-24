using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fidelizacion.Domain.Model
{
    public class Extractos
    {
        public string? Id { get; set; }

        public DateTime? Fecha { get; set; }

        public string? Descripcion { get; set; }

        public float? ValorMovimiento { get; set; }

        public string? OrigenMovimiento { get; set; }

        public Usuario? Usuario { get; set; }
    }
}
