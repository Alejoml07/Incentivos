using PuntosLeonisa.Products.Domain.Service.DTO.Banners;
using PuntosLeonisa.Products.Domain.Service.DTO.Genericos;
using PuntosLeonisa.Products.Domain.Service.DTO.Productos;
using System;
namespace PuntosLeonisa.Products.Domain.Interfaces
{
    public interface IProductoRepository : IRepository<Producto>
    {
        //Task<PagedResult<Producto>> GetProductsByFiltersAndRange(ProductosFilters filter);
        Task<PagedResult<IGrouping<string, Producto>>> GetProductsByFiltersAndRange(ProductosFilters queryObject);
        Task<PagedResult<IGrouping<string, Producto>>> GetProductsForSearch(SearchDto data);
        Task<PagedResult<IGrouping<string, Producto>>> GetProductsForSearchAll(SearchDto data);
        Task<FiltroDto> ObtenerFiltros(GeneralFiltersWithResponseDto generalFiltersWithResponseDto);
        Task<IEnumerable<Producto>> GetByRef(string referencia);
        Task<IEnumerable<Producto>> GetProductByProveedor(string nit);
        Task<IEnumerable<Producto>> GetProductByProveedorOrAll(TipoUsuarioDto[] data);
        Task<IEnumerable<Producto>> GetProductByName(string nombre);

    }
}


