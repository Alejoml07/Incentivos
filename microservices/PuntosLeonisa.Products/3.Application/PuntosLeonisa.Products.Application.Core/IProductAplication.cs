using PuntosLeonisa.Products.Application.Core.Interfaces;
using PuntosLeonisa.Products.Domain.Service.DTO.Genericos;
using PuntosLeonisa.Products.Domain.Service.DTO.Productos;
using PuntosLeonisa.Products.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Products.Application.Core;

public interface IProductApplication : IApplicationCore<ProductoDto>
{
    Task<GenericResponse<bool>> AddProductoInventario(ProductoInventarioDto[] products);
    Task<GenericResponse<bool>> AddProductoPrecios(ProductoPreciosDto[] productoPrecios);
    Task<GenericResponse<FiltroDto>> ObtenerFiltros(GeneralFiltersWithResponseDto generalFiltersWithResponseDto);
    Task<GenericResponse<PagedResult<ProductoDto>>> GetProductosByFiltersAndRange(ProductosFilters filtros);
    Task<GenericResponse<GeneralFiltersWithResponseDto>> GetAndApplyFilters(GeneralFiltersWithResponseDto filtrosDto);
}

