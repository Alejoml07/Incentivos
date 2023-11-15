using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Products.Domain.Service.DTO.Productos
{
    public class FiltroDto
    {
        public List<string> CategoriaNombre { get; set; }
        public List<int> Puntos { get; set; }
        public List<string> SubCategoriaNombre { get; set; }
        public List<string> Marca { get; set; }

    }
}
