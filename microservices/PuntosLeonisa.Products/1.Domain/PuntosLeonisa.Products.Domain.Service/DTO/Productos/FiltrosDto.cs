using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Products.Domain.Service.DTO.Productos
{
    public class FiltroDto
    {
        public List<Categoria> Categorias { get; set; }
        public int PuntosMin { get; set; } 
        public int PuntosMax { get; set; }
        public int Precio { get; set; }
        public List<string> SubCategoriaNombre { get; set; }
        public List<string> Marca { get; set; } 

    }

    public class Categoria
    {
        public string CategoriaNombre { get; set; }
        public List<string> Subcategorias { get; set; }
    }
}
