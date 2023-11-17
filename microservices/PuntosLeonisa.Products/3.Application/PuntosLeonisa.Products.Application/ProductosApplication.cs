
using AutoMapper;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using Newtonsoft.Json.Linq;
using PuntosLeonisa.Products.Application.Core;
using PuntosLeonisa.Products.Domain;
using PuntosLeonisa.Products.Domain.Interfaces;
using PuntosLeonisa.Products.Domain.Service.DTO.Genericos;
using PuntosLeonisa.Products.Domain.Service.DTO.Productos;
using PuntosLeonisa.Products.Infrasctructure.Common;
using PuntosLeonisa.Products.Infrasctructure.Common.Communication;
using PuntosLeonisa.Products.Infrasctructure.Repositorie;

namespace PuntosLeonisa.Products.Application;

public class ProductosApplication : IProductApplication
{
    private readonly IMapper mapper;
    private readonly IProductoRepository productoRepository;
    private readonly GenericResponse<ProductoDto> response;

    public ProductosApplication(IMapper mapper, IProductoRepository productoRepository)
    {
        if (productoRepository is null)
        {
            throw new ArgumentNullException(nameof(productoRepository));
        }

        this.mapper = mapper;
        this.productoRepository = productoRepository;
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

    public async Task<GenericResponse<ProductoDto[]>> AddRange(ProductoDto[] value)
    {
        try
        {
            var productos = this.mapper.Map<Producto[]>(value);

            foreach (var producto in productos)
            {
                producto.Id = Guid.NewGuid().ToString();
            }

            await this.productoRepository.AddRange(productos);
            var responseOnly = new GenericResponse<ProductoDto[]>
            {
                Result = value
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

            return new GenericResponse<PagedResult<ProductoDto>>()
            {
                Result = this.mapper.Map<PagedResult<ProductoDto>>(response)
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


}