using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Products.Domain.Service.DTO.Productos
{
    public class ProductosFilters
    {
        public string? TipoUsuario { get; set; }
        public string OrderMode { get; set; } = "ASC";
        public string OrderBy { get; set; } = "Nombre";
        public string Search { get; set; } = "";
        public string MaxRangePropertyNameEnd { get; set; } = "Max";
        public string MinRangePropertyNameEnd { get; set; } = "Min";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int PageCount { get; set; }
        public Dictionary<string, dynamic> Filters { get; set; }
        public ProductosFilters() { }


    }
}
