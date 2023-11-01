
using AutoMapper;
using PuntosLeonisa.Products.Application.Core;
using PuntosLeonisa.Products.Domain;
using PuntosLeonisa.Products.Domain.Interfaces;
using PuntosLeonisa.Products.Domain.Service.DTO.Productos;
using PuntosLeonisa.Products.Infrasctructure.Common;
using PuntosLeonisa.Products.Infrasctructure.Common.Communication;

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

            //antes de guardar se debe subir la imagen
            await UploadImageToProducts(value);
            var producto = this.mapper.Map<Producto>(value);
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
            var ToDelete = await this.GetById(id) ?? throw new ArgumentException("Producto no encontrado");
            var productoToDelete = this.mapper.Map<Producto>(ToDelete.Result);
            await this.productoRepository.Delete(productoToDelete);

            return ToDelete;
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

    public async  Task<GenericResponse<bool>> AddProductoPrecios(ProductoPreciosDto[] productoPrecios)
    {
        try
        {
            foreach (var producto in productoPrecios)
            {
                var productoExist = await this.productoRepository.GetById(producto.EAN ?? "");
                productoExist.Precio = producto.Precio;
                productoExist.PrecioOferta = producto.Precio;
                await this.productoRepository.Update(productoExist);
            }

            return new GenericResponse<bool>() { Result = true };
        }
        catch (Exception)
        {

            throw;
        }
    }
}