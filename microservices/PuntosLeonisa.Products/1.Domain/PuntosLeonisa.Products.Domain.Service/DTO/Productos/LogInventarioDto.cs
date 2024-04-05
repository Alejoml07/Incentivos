using PuntosLeonisa.Products.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Products.Domain.Service.DTO.Productos
{
    public class LogInventarioDto
    {
        public string? Id { get; set; }

        public string? EAN { get; set; }

        public int? Cantidad { get; set; }

        public DateTime? FechaActualizacion { get; set; }

        public string? Usuario { get; set; }


    }
}
