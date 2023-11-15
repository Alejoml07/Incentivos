using PuntosLeonisa.Products.Domain.Service.DTO.Genericos;
using PuntosLeonisa.Products.Domain.Service.DTO.Productos;
using System;
namespace PuntosLeonisa.Products.Domain.Interfaces
{
    public interface IProductoRepository : IRepository<Producto>
    {
        Task<PagedResult<Producto>> GetProductsByFiltersAndRange(ProductosFilters filter);
        Task<FiltroDto> ObtenerFiltros();

    }
}


