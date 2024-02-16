
using AutoMapper;
using PuntosLeonisa.Infrasctructure.Core.ExternalServiceInterfaces;
using PuntosLeonisa.Products.Application.Core;
using PuntosLeonisa.Products.Domain;
using PuntosLeonisa.Products.Domain.Interfaces;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Products.Domain.Service.DTO.Genericos;
using PuntosLeonisa.Products.Domain.Service.DTO.Productos;
using PuntosLeonisa.Products.Infrasctructure.Common;
using PuntosLeonisa.Products.Infrasctructure.Common.Communication;
namespace PuntosLeonisa.Products.Application;

public class ProductosApplication : IProductApplication
{
    private readonly IMapper mapper;
    private readonly IProductoRepository productoRepository;
    private readonly IProveedorExternalService proveedorExternalService;
    private readonly GenericResponse<ProductoDto> response;

    public ProductosApplication(IMapper mapper, IProductoRepository productoRepository, IProveedorExternalService proveedorExternalService)
    {
        if (productoRepository is null)
        {
            throw new ArgumentNullException(nameof(productoRepository));
        }
        this.mapper = mapper;
        this.productoRepository = productoRepository;
        this.proveedorExternalService = proveedorExternalService;
        this.response = new GenericResponse<ProductoDto>();
    }

    public async Task<GenericResponse<ProductoDto>> Add(ProductoDto value)
    {
        try
        {
            //TODO: Hacer las validaciones
            var productoExist = await this.productoRepository.GetById(value.EAN ?? string.Empty);
            var parametroEquivalenciaEnPuntos = 87;
            if (productoExist != null)
            {
                this.mapper.Map(value, productoExist);
                await this.productoRepository.Update(productoExist);
                return this.response;
            }
            //antes de guardar se debe subir la imagen
            await UploadImageToProducts(value);
            var producto = this.mapper.Map<Producto>(value);
            //TODO: Colocar el parametro de puntos y su equivalencia 87
            producto.Id = Guid.NewGuid().ToString();
            producto.ProveedorLite = this.proveedorExternalService.GetProveedorByNit(producto.Proveedor).GetAwaiter().GetResult().Result;
            if (producto.ProveedorLite == null)
            {
                throw new Exception("El proveedor no existe");
            }
            producto.Puntos = ((int?)(producto.Precio / parametroEquivalenciaEnPuntos));
            await this.productoRepository.Add(producto);
            this.response.Result = value;
            return this.response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private static async Task UploadImageToProducts(ProductoDto producto)
    {
        var azureHelper = new AzureHelper("DefaultEndpointsProtocol=https;AccountName=stgactincentivos;AccountKey=mtBoBaUJu8BKcHuCfdWzk1au7Upgif0rlzD+BlfAJZBsvQ02CiGzCNG5gj1li10GF8RpUwz6h+Mj+AStMOwyTA==;EndpointSuffix=core.windows.net");
        if (!string.IsNullOrEmpty(producto.UrlImagen1))
        {
            byte[] bytes = Convert.FromBase64String(producto.UrlImagen1);
            producto.UrlImagen1 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
        }
        if (!string.IsNullOrEmpty(producto.UrlImagen2))
        {
            byte[] bytes = Convert.FromBase64String(producto.UrlImagen2);
            producto.UrlImagen2 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
        }
        if (!string.IsNullOrEmpty(producto.UrlImagen3))
        {
            byte[] bytes = Convert.FromBase64String(producto.UrlImagen3);
            producto.UrlImagen3 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
        }
        if (!string.IsNullOrEmpty(producto.UrlImagen4))
        {
            byte[] bytes = Convert.FromBase64String(producto.UrlImagen4);
            producto.UrlImagen4 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
        }
        if (!string.IsNullOrEmpty(producto.UrlImagen5))
        {
            byte[] bytes = Convert.FromBase64String(producto.UrlImagen5);
            producto.UrlImagen5 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
        }
    }

    public async Task<GenericResponse<Tuple<ProductoDto[], List<string>>>> AddRangeProducts(ProductoDto[] value)
    {
        var errores = new List<string>();
        try
        {
            var productos = this.mapper.Map<Producto[]>(value);
            var productosProcesados = new List<Producto>();
            foreach (var producto in productos)
            {
                producto.ProveedorLite = this.proveedorExternalService.GetProveedorByNit(producto.Proveedor).GetAwaiter().GetResult().Result;
                if (producto.ProveedorLite == null)
                {
                    errores.Add($"El producto {producto.EAN} tiene un  numero de proveedor que no existe : {producto.Proveedor}");
                }
                else
                {
                    var productoExist = await this.productoRepository.GetById(producto.EAN ?? string.Empty);
                    if (productoExist != null)
                    {
                        if (productoExist.ProveedorLite == null)
                            productoExist.ProveedorLite = producto.ProveedorLite;
                            this.mapper.Map(producto, productoExist);
                            await this.productoRepository.Update(productoExist);
                    }
                    else
                    {
                        producto.Id = Guid.NewGuid().ToString();                        
                        await this.productoRepository.Add(producto);
                    }
                    productosProcesados.Add(producto);
                }
            }
            var productosProcesadosDto = this.mapper.Map<ProductoDto[]>(productosProcesados);   
            var responseOnly = new GenericResponse<Tuple<ProductoDto[], List<string>>>
            {
                Result = new Tuple<ProductoDto[], List<string>>(productosProcesadosDto, errores)
            };
            return responseOnly;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<GenericResponse<ProductoDto>> Delete(ProductoDto value)
    {
        throw new NotImplementedException();
    }

    public async Task<GenericResponse<ProductoDto>> DeleteById(string id)
    {
        try
        {
            var productoToDelete = await this.productoRepository.GetById(id) ?? throw new Exception("El producto no existe");
            this.response.Result = this.mapper.Map<ProductoDto>(productoToDelete);
            await this.productoRepository.Delete(productoToDelete);
            return this.response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<ProductoDto>>> GetAll()
    {
        var productos = await this.productoRepository.GetAll();
        var productoDto = this.mapper.Map<ProductoDto[]>(productos);
        var responseOnly = new GenericResponse<IEnumerable<ProductoDto>>
        {
            Result = productoDto
        };
        return responseOnly;
    }

    public async Task<GenericResponse<ProductoDto>> GetById(string id)
    {
        var responseRawData = await this.productoRepository.GetById(id);
        var responseData = this.mapper.Map<ProductoDto>(responseRawData);
        this.response.Result = responseData;
        return this.response;
    }

    public async Task<GenericResponse<IEnumerable<ProductoDto>>> GetByRef(string referencia)
    {
        var responseRawData = await this.productoRepository.GetByRef(referencia);
        var responseData = this.mapper.Map<IEnumerable<ProductoDto>>(responseRawData);
        var newResponse = new GenericResponse<IEnumerable<ProductoDto>>();
        newResponse.Result = responseData;
        return newResponse;
    }

    public async Task<GenericResponse<ProductoDto>> Update(ProductoDto value)
    {
        try
        {
            var response = await this.productoRepository.GetById(value.EAN ?? "");
            if (response != null)
            {
                this.mapper.Map(value, response);
                await this.productoRepository.Update(response);
            }
            this.response.Result = value;
            return this.response;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<bool>> AddProductoInventario(ProductoInventarioDto[] productos)
    {
        try
        {
            foreach (var producto in productos)
            {
                var productoExist = await this.productoRepository.GetById(producto.EAN ?? "");
                if (productoExist == null)
                {
                    continue;
                }
                productoExist.Cantidad = producto.Cantidad;
                await this.productoRepository.Update(productoExist);
            }

            return new GenericResponse<bool>() { Result = true };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<bool>> AddProductoPrecios(ProductoPreciosDto[] productoPrecios)
    {
        try
        {
            foreach (var producto in productoPrecios)
            {
                var productoExist = await this.productoRepository.GetById(producto.EAN ?? "");
                if (productoExist == null)
                {
                    continue;
                }
                productoExist.Precio = producto.Precio;
                //TODO: Colocar el parametro de puntos y su equivalencia 87
                productoExist.Puntos = (int)Math.Round((float)(producto.Precio / 87));
                productoExist.PrecioOferta = producto.PrecioOferta;
                await this.productoRepository.Update(productoExist);
            }
            return new GenericResponse<bool>() { Result = true };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<PagedResult<ProductoDto>>> GetProductosByFiltersAndRange(ProductosFilters filtros)
    {
        try
        {
            var response = await this.productoRepository.GetProductsByFiltersAndRange(filtros);
            var productos = response.Data.SelectMany(group => group.ToList()).ToList();
            var pageresult = new PagedResult<Producto>();
            pageresult.PageNumber = response.PageNumber;
            pageresult.PageSize = response.PageSize;
            pageresult.TotalCount = response.TotalCount;
            pageresult.Data = productos;
            return new GenericResponse<PagedResult<ProductoDto>>()
            {
                Result = this.mapper.Map<PagedResult<ProductoDto>>(pageresult)
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<GenericResponse<FiltroDto>> ObtenerFiltros(GeneralFiltersWithResponseDto generalFiltersWithResponseDto)
    {
        var filtros = await this.productoRepository.ObtenerFiltros(generalFiltersWithResponseDto);
        var responseOnly = new GenericResponse<FiltroDto>
        {
            Result = filtros
        };
        return responseOnly;
    }

    public async Task<GenericResponse<GeneralFiltersWithResponseDto>> GetAndApplyFilters(GeneralFiltersWithResponseDto generalFiltersWithResponseDto)
    {
        try
        {
            PagedResult<ProductoDto>? productosResponse = null;
            if (generalFiltersWithResponseDto?.ApplyFiltro != null)
            {
                // Utilizar ApplyFiltro para obtener los productos
                var resultApplied = await this.GetProductosByFiltersAndRange(generalFiltersWithResponseDto?.ApplyFiltro);
                productosResponse = resultApplied.Result;
            }
            else
            {
                var response = await this.GetAll();
                productosResponse = new PagedResult<ProductoDto>()
                {
                    PageNumber = 1,
                    Data = response.Result,
                    PageSize = 10,
                    TotalCount = response.Result.Count()
                };
            }
            // Opcional: Determinar o ajustar filtros adicionales basados en productos obtenidos
            // ..
            // Obtener o ajustar filtros usando GetFiltro
            var filtrosResponse = await this.ObtenerFiltros(generalFiltersWithResponseDto); // O alguna lógica que involucre GetFiltro
            // Combinar productos y filtros en una respuesta
            return new GenericResponse<GeneralFiltersWithResponseDto>
            {
                Result = new GeneralFiltersWithResponseDto
                {
                    DataByFilter = productosResponse,
                    FiltrosFromProductos = filtrosResponse.Result // Ajustar según la lógica de negocio
                }
            };
        }
        catch (Exception)
        {
            // Manejar excepciones
            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<bool>>> UpdateInventory(ProductoRefence[] data)
    {
        try
        {
            foreach (var producto in data)
            {
                var productos = await this.productoRepository.GetById(producto.EAN);
                productos.Cantidad -= producto.Quantity;
                await this.productoRepository.Update(productos);
            }
            return new GenericResponse<IEnumerable<bool>>()
            {
                Result = new List<bool>() { true }
            };
        }
        catch (Exception)
        {
            throw;
        }
    }

    public Task<GenericResponse<ProductoDto[]>> AddRange(ProductoDto[] value)
    {
        throw new NotImplementedException();
    }

    public Task<GenericResponse<ProductoRefence>> GetProductByEAN(string ean)
    {
        var response = this.productoRepository.GetById(ean);
        var responseDto = this.mapper.Map<ProductoRefence>(response);
        return Task.FromResult(new GenericResponse<ProductoRefence>
        {
            Result = responseDto
        });
    }
}
