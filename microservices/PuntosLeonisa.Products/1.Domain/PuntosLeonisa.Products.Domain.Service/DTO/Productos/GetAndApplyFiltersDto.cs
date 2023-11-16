using PuntosLeonisa.Products.Domain.Service.DTO.Genericos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Products.Domain.Service.DTO.Productos
{
    public class GeneralFiltersWithResponseDto
    {
        public FiltroDto FiltrosFromProductos { get; set; }
        public ProductosFilters ApplyFiltro { get; set; }

        public PagedResult<ProductoDto> DataByFilter { get; set; }

    }
}
