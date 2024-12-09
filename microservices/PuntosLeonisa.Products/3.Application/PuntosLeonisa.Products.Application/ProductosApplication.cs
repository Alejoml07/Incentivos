
using AutoMapper;
using PuntosLeonisa.Infrasctructure.Core.ExternalServiceInterfaces;
using PuntosLeonisa.Products.Application.Core;
using PuntosLeonisa.Products.Domain;
using PuntosLeonisa.Products.Domain.Interfaces;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Products.Domain.Service.DTO.Banners;
using PuntosLeonisa.Products.Domain.Service.DTO.Genericos;
using PuntosLeonisa.Products.Domain.Service.DTO.Productos;
using PuntosLeonisa.Products.Domain.Service.Interfaces;
using PuntosLeonisa.Products.Infrasctructure.Common;
using PuntosLeonisa.Products.Infrasctructure.Common.Communication;
namespace PuntosLeonisa.Products.Application;

public class ProductosApplication : IProductApplication
{
    private readonly IMapper mapper;
    private readonly IProductoRepository productoRepository;
    private readonly ILogInventarioRepository logInventarioRepository;
    private readonly IBannerRepository bannerRepository;
    private readonly IProveedorExternalService proveedorExternalService;
    private readonly GenericResponse<ProductoDto> response;

    public ProductosApplication(IMapper mapper, IProductoRepository productoRepository, IProveedorExternalService proveedorExternalService, IBannerRepository bannerRepository, ILogInventarioRepository logInventarioRepository)
    {
        if (productoRepository is null)
        {
            throw new ArgumentNullException(nameof(productoRepository));
        }
        this.mapper = mapper;
        this.productoRepository = productoRepository;
        this.bannerRepository = bannerRepository;
        this.logInventarioRepository = logInventarioRepository;
        this.proveedorExternalService = proveedorExternalService;
        this.response = new GenericResponse<ProductoDto>();
        this.logInventarioRepository = logInventarioRepository;
    }

    public async Task<GenericResponse<ProductoDto>> Add(ProductoDto value)
    {
        try
        {
            //TODO: Hacer las validaciones
            var productoExist = await this.productoRepository.GetById(value.EAN ?? string.Empty);
            var parametroEquivalenciaEnPuntos = 85;
            if (productoExist != null)
            {
                value.ProveedorLite= this.proveedorExternalService.GetProveedorByNit(value.Proveedor).GetAwaiter().GetResult().Result;
                this.mapper.Map(value, productoExist);
                await this.productoRepository.Update(productoExist);
                return this.response;
            }
            //antes de guardar se debe subir la imagen
            await UploadImageToProducts(value);
            var producto = this.mapper.Map<Producto>(value);
            //TODO: Colocar el parametro de puntos y su equivalencia 85
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

    private static async Task UploadImageToBanner(Banner banner)
    {   
        var azureHelper = new AzureHelper("DefaultEndpointsProtocol=https;AccountName=stgactincentivos;AccountKey=mtBoBaUJu8BKcHuCfdWzk1au7Upgif0rlzD+BlfAJZBsvQ02CiGzCNG5gj1li10GF8RpUwz6h+Mj+AStMOwyTA==;EndpointSuffix=core.windows.net");
        if (!string.IsNullOrEmpty(banner.Imagen1))
        {
            if(!banner.Imagen1.Contains("https://stgactincentivos.blob.core.windows.net"))
            {
                byte[] bytes = Convert.FromBase64String(banner.Imagen1);
                banner.Imagen1 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }          
        }
        if (!string.IsNullOrEmpty(banner.Imagen2))
        {
            if (!banner.Imagen2.Contains("https://stgactincentivos.blob.core.windows.net"))
            {
                byte[] bytes = Convert.FromBase64String(banner.Imagen2);
                banner.Imagen2 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }
        }
        if (!string.IsNullOrEmpty(banner.Imagen3))
        {
            if (!banner.Imagen3.Contains("https://stgactincentivos.blob.core.windows.net"))
            {
                byte[] bytes = Convert.FromBase64String(banner.Imagen3);
                banner.Imagen3 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }
        }
        if (!string.IsNullOrEmpty(banner.Imagen4))
        {
            if (!banner.Imagen4.Contains("https://stgactincentivos.blob.core.windows.net"))
            {
                byte[] bytes = Convert.FromBase64String(banner.Imagen4);
                banner.Imagen4 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }
        }
        if (!string.IsNullOrEmpty(banner.Imagen5))
        {
            if (!banner.Imagen5.Contains("https://stgactincentivos.blob.core.windows.net"))
            {
                byte[] bytes = Convert.FromBase64String(banner.Imagen5);
                banner.Imagen5 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }
        }
        if (!string.IsNullOrEmpty(banner.Imagen6))
        {
            if (!banner.Imagen6.Contains("https://stgactincentivos.blob.core.windows.net"))
            {
                byte[] bytes = Convert.FromBase64String(banner.Imagen6);
                banner.Imagen6 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }
        }
        if (!string.IsNullOrEmpty(banner.Imagen7))
        {
            if (!banner.Imagen7.Contains("https://stgactincentivos.blob.core.windows.net"))
            {
                byte[] bytes = Convert.FromBase64String(banner.Imagen7);
                banner.Imagen7 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }
        }
        if (!string.IsNullOrEmpty(banner.Imagen8))
        {
            if (!banner.Imagen8.Contains("https://stgactincentivos.blob.core.windows.net"))
            {
                byte[] bytes = Convert.FromBase64String(banner.Imagen8);
                banner.Imagen8 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }
        }
        if (!string.IsNullOrEmpty(banner.Imagen9))
        {
            if (!banner.Imagen9.Contains("https://stgactincentivos.blob.core.windows.net"))
            {
                byte[] bytes = Convert.FromBase64String(banner.Imagen9);
                banner.Imagen9 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }
        }
        if (!string.IsNullOrEmpty(banner.Imagen10))
        {
            if (!banner.Imagen10.Contains("https://stgactincentivos.blob.core.windows.net"))
            {
                byte[] bytes = Convert.FromBase64String(banner.Imagen10);
                banner.Imagen10 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }
        }
        if (!string.IsNullOrEmpty(banner.Imagen11))
        {
            if (!banner.Imagen11.Contains("https://stgactincentivos.blob.core.windows.net"))
            {
                byte[] bytes = Convert.FromBase64String(banner.Imagen11);
                banner.Imagen11 = await azureHelper.UploadFileToBlobAsync(bytes, ".webp", "image/webp");
            }
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
                        {
                            productoExist.ProveedorLite = producto.ProveedorLite;
                        }                            
                            this.mapper.Map(productoExist, producto);
                            producto.Precio = productoExist.Precio;
                            producto.PrecioOferta = productoExist.PrecioOferta;
                            producto.Puntos = productoExist.Puntos;
                            await this.productoRepository.Update(producto);
                    }
                    else
                    {
                        producto.Id = Guid.NewGuid().ToString();
                        producto.FechaCreacion = DateTime.Now;  
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
            Result = productoDto.OrderByDescending(x => x.FechaCreacion)
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
            var logInventario = new LogInventarioDto();
            foreach (var producto in productos)
            {
                var productoExist = await this.productoRepository.GetById(producto.EAN ?? "");
                if (productoExist == null)
                {
                    continue;
                }
                productoExist.Cantidad = producto.Cantidad;
                logInventario.Id = Guid.NewGuid().ToString();
                logInventario.Cantidad = producto.Cantidad;
                logInventario.EAN = producto.EAN;
                if(producto.Email == null || producto.Email == "")
                {
                    producto.Email = "LEONISA";
                }
                logInventario.Usuario = producto.Email;
                logInventario.FechaActualizacion = DateTime.Now;
                await this.logInventarioRepository.Add(logInventario);
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
                productoExist.PuntosSinDescuento = (int)Math.Round((float)(producto.Precio / 85));
                if (productoExist.ProveedorLite.Descuento != null)
                {
                    productoExist.Precio = producto.Precio - ((producto.Precio * productoExist.ProveedorLite.Descuento) / 100);
                }
                productoExist.Puntos = (int)Math.Round((float)(productoExist.Precio / 85));
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
            var productos = response.Data
            .SelectMany(group => group) 
            .GroupBy(p => p.Referencia)  
            .Select(g => g.FirstOrDefault()) 
            .Where(p => p != null && p.Cantidad > 0) 
            .ToList(); 
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
                if(producto.ProveedorLite.Nit == "811044814")
                {
                    continue;
                }
                else
                {
                    var productos = await this.productoRepository.GetById(producto.EAN);
                    if (producto.Quantity == null || producto.Quantity == 0)
                    {
                        continue;
                    }
                    productos.Cantidad -= producto.Quantity;
                    await this.productoRepository.Update(productos);
                }
                
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

    public async Task<GenericResponse<ProductoRefence>> GetProductByEAN(string ean)
    {
        try
        {
            var response = await this.productoRepository.GetById(ean);
            var responseDto = this.mapper.Map<ProductoRefence>(response);
            return new GenericResponse<ProductoRefence>()
            {
                Result = responseDto
            };
        }
        catch (Exception)
        {

            throw;
        }
        
    }
    public async Task<GenericResponse<IEnumerable<ProductoDto>>> GetProductByProveedor(string nit)
    {
        try
        {
            var responseRawData = await this.productoRepository.GetProductByProveedor(nit);
            var responseData = this.mapper.Map<IEnumerable<ProductoDto>>(responseRawData);
            var newResponse = new GenericResponse<IEnumerable<ProductoDto>>();
            newResponse.Result = responseData;
            return newResponse;
        }
        catch (Exception)
        {

            throw;
        }
        
    }

    public async Task<GenericResponse<bool>> AddBannerEventos(Banner banner)
    {
        try
        {
            var bannerExist = await this.bannerRepository.GetById(banner.TipoUsuario);
            if (bannerExist != null)
            {
                await UploadImageToBanner(banner);
                await this.bannerRepository.Update(banner);
                return new GenericResponse<bool>()
                {
                    Result = true
                };
            }
            else
            {
                await UploadImageToBanner(banner);
                await this.bannerRepository.Add(banner);
                return new GenericResponse<bool>()
                {
                    Result = true
                };
            }
        }
        catch (Exception)
        {

            throw;
        }        
    }

    public async Task<GenericResponse<bool>> AddBanner(Banner banner)
    {
        try
        {
            var bannerExist = await this.bannerRepository.GetById(banner.TipoUsuario);
            if (bannerExist != null)
            {
                await UploadImageToBanner(banner);
                await this.bannerRepository.Update(banner);
                return new GenericResponse<bool>()
                {
                    Result = true
                };
            }
            else
            {
                await UploadImageToBanner(banner);
                await this.bannerRepository.Add(banner);
                return new GenericResponse<bool>()
                {
                    Result = true
                };
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

        public Task<GenericResponse<Banner>> GetBannerById(Banner data)
        {
            try
            {
                var response = this.bannerRepository.GetBannersByUserType(data);
                return Task.FromResult(new GenericResponse<Banner>()
                {
                    Result = response.Result
                });
            }
            catch (Exception)
            {

                throw;
            }
        }

    public Task<GenericResponse<Banner>> GetBannerByEvent(Banner data)
    {
        try
        {
            var response = this.bannerRepository.GetBannersByUserType(data);
            return Task.FromResult(new GenericResponse<Banner>()
            {
                Result = response.Result
            });
        }
        catch (Exception)
        {

            throw;
        }
    }
    public async Task<GenericResponse<IEnumerable<ProductoDto>>> GetProductByProveedorOrAll(TipoUsuarioDto[] data)
    {
        try
        {
            var responseRawData = await this.productoRepository.GetProductByProveedorOrAll(data);
            var responseData = this.mapper.Map<IEnumerable<ProductoDto>>(responseRawData);
            var newResponse = new GenericResponse<IEnumerable<ProductoDto>>();
            newResponse.Result = responseData.OrderByDescending(x => x.FechaCreacion); ;
            return newResponse;
        }
        catch (Exception)
        {

            throw;
        }

    }

    public async Task<GenericResponse<PagedResult<ProductoDto>>> GetProductsBySearch(SearchDto data)
    {
        try
        {
            if(data.Busqueda == null || data.Busqueda == "")
            {
                //si busqueda es nulo o vacio se retornan todos los productos
                var response2 = await this.productoRepository.GetProductsForSearchAll(data);
                // ayudame a agruparlo por referencia
                var productos2 = response2.Data.SelectMany(group => group.ToList()).ToList().GroupBy(p => p.Referencia).Select(g => g.FirstOrDefault());
                var pageresult2 = new PagedResult<Producto>();
                pageresult2.PageNumber = response2.PageNumber;
                pageresult2.PageSize = response2.PageSize;
                pageresult2.TotalCount = response2.TotalCount;
                pageresult2.Data = productos2;
                return new GenericResponse<PagedResult<ProductoDto>>()
                {
                    Result = this.mapper.Map<PagedResult<ProductoDto>>(pageresult2)
                };

            }
            else
            {
                var response = await this.productoRepository.GetProductsForSearch(data);
                var productos = response.Data.SelectMany(group => group.ToList()).ToList().GroupBy(p => p.Referencia).Select(g => g.FirstOrDefault());
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
            
        }
        catch (Exception)
        {

            throw;
        }
        
    }

    public async Task<GenericResponse<IEnumerable<ProductoDto>>> GetProductByName(string nombre)
    {
        try
        {
            var producto = await this.productoRepository.GetProductByName(nombre);
            var response = this.mapper.Map<IEnumerable<ProductoDto>>(producto);
            return new GenericResponse<IEnumerable<ProductoDto>>()
            {
                Result = response
            };
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<GenericResponse<IEnumerable<bool>>> DeleteLeonisaProduct()
    {
        var productos = await this.productoRepository.GetProductByProveedor("811044814");
        foreach (var producto in productos)
        {
            await this.productoRepository.Delete(producto);
        }
        return new GenericResponse<IEnumerable<bool>>()
        {
            Result = new List<bool>() { true }
        };
    }
}
