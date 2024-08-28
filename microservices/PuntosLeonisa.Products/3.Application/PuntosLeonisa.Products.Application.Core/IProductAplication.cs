using PuntosLeonisa.Products.Application.Core.Interfaces;
using PuntosLeonisa.Products.Domain;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Products.Domain.Service.DTO.Banners;
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
    Task<GenericResponse<IEnumerable<ProductoDto>>> GetByRef(string referencia);
    Task<GenericResponse<IEnumerable<bool>>> UpdateInventory(ProductoRefence[] data);
    Task<GenericResponse<Tuple<ProductoDto[], List<string>>>> AddRangeProducts(ProductoDto[] value);
    Task<GenericResponse<ProductoRefence>> GetProductByEAN(string ean);
    Task<GenericResponse<IEnumerable<ProductoDto>>> GetProductByProveedor(string proveedor);
    Task<GenericResponse<bool>> AddBanner(Banner banner);
    Task<GenericResponse<Banner>> GetBannerById(Banner data);
    Task<GenericResponse<bool>> AddBannerEventos(Banner banner);
    Task<GenericResponse<Banner>> GetBannerByEvent(Banner data);
    Task<GenericResponse<IEnumerable<ProductoDto>>> GetProductByProveedorOrAll(TipoUsuarioDto[] data);
    Task<GenericResponse<PagedResult<ProductoDto>>> GetProductsBySearch(SearchDto data);
    Task<GenericResponse<IEnumerable<ProductoDto>>> GetProductByName(string nombre);
    Task<GenericResponse<IEnumerable<bool>>> DeleteLeonisaProduct();
}

