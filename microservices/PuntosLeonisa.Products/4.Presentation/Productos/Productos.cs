

namespace Productos
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using PuntosLeonisa.Products.Application.Core;
    using PuntosLeonisa.Products.Infrasctructure.Common.Communication;
    using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
    using System.Net;
    using PuntosLeonisa.Products.Domain.Service.DTO.Productos;
    using Microsoft.Extensions.Primitives;
    using System.Linq;
    using PuntosLeonisa.Products.Domain.Model;
    using PuntosLeonisa.Products.Domain.Service.DTO.Banners;

    public class Productos
    {
        private readonly IProductApplication productoApplication;
        private readonly GenericResponse<ProductoDto> responseError;
        private readonly BadRequestObjectResult productoApplicationErrorResult;

        public Productos(IProductApplication productoApplication)
        {
            this.productoApplication = productoApplication;
            this.responseError = new GenericResponse<ProductoDto>();
            this.productoApplicationErrorResult = new BadRequestObjectResult(this.responseError);
        }

        [FunctionName("Productos")]
        [OpenApiOperation(operationId: "Productos", tags: new[] { "Productos/Producto" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Guarda el producto")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Productos/Producto")] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Product:GetProductos Inicia obtener todos los productos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<ProductoDto>(requestBody);
                await this.productoApplication.Add(data);
                return new OkResult();
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetProductos")]
        [OpenApiOperation(operationId: "GetProductos", tags: new[] { "Productos/GetProductos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]
        public async Task<IActionResult> GetProductos(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Productos/GetProductos")] HttpRequest req,
        ILogger log)
        {
            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }
                log.LogInformation($"Product:GetProductos Inicia obtener todos los productos. Fecha:{DateTime.UtcNow}");
                var productos = await productoApplication.GetAll();
                log.LogInformation($"Product:GetProductos finaliza obtener todos los productos sin errores. Fecha:{DateTime.UtcNow}");
                return new OkObjectResult(productos);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        private IActionResult GetFunctionError(ILogger log, string logMessage, Exception ex)
        {
            log.LogError(ex, logMessage, null);
            this.responseError.Message = ex.Message;
            this.responseError.IsSuccess = false;
            return this.productoApplicationErrorResult;
        }

        [FunctionName("UploadImageToBlob")]
        public static async Task<IActionResult> UploadImageToBlob(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            _ = await req.ReadFormAsync();
            var file = req.Form.Files["image"];

            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("File missing");
            }

            string connectionString = "DefaultEndpointsProtocol=https;AccountName=stgactincentivos;AccountKey=mtBoBaUJu8BKcHuCfdWzk1au7Upgif0rlzD+BlfAJZBsvQ02CiGzCNG5gj1li10GF8RpUwz6h+Mj+AStMOwyTA==;EndpointSuffix=core.windows.net";
            string containerName = "$web";
            string blobName = "/img/" + Path.GetRandomFileName() + Path.GetExtension(file.FileName);

            BlobServiceClient blobServiceClient = new(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            string mimeType = file.ContentType;

            BlobUploadOptions uploadOptions = new()
            {
                HttpHeaders = new BlobHttpHeaders { ContentType = mimeType }
            };

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, uploadOptions);
            }

            string blobUrl = blobClient.Uri.ToString();

            return new OkObjectResult(new { message = $"Imagen {blobName} subida exitosamente.", url = blobUrl });

        }

        [FunctionName("LoadProducts")]
        [OpenApiOperation(operationId: "LoadProducts", tags: new[] { "Productos/LoadProducts" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]
        public async Task<IActionResult> LoadProducts(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Productos/LoadProducts")] HttpRequest req,
           ILogger log)
        {
            try
            {
                log.LogInformation($"Product:LoadProducts Inicia agregar productos masivos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var products = JsonConvert.DeserializeObject<ProductoDto[]>(requestBody);
                var result = await this.productoApplication.AddRangeProducts(products);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetProduct")]
        [OpenApiOperation(operationId: "GetProduct", tags: new[] { "Productos/GetProduct" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]
        public async Task<IActionResult> GetProduct(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Productos/GetProduct/{id}")] HttpRequest req,
           string id,  // <-- Parámetro adicional
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }

                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException($"'{nameof(id)}' cannot be null or empty.", nameof(id));
                }

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }

                var producto = await this.productoApplication.GetById(id);

                return new OkObjectResult(producto);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }


        [FunctionName("DeleteProduct")]
        [OpenApiOperation(operationId: "DeleteProduct", tags: new[] { "Productos/DeleteProduct" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]

        public async Task<IActionResult> DeleteProduct(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Productos/DeleteProduct/{id}")] HttpRequest req,
        string id,  // <-- Parámetro adicional
        ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }

                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException($"'{nameof(id)}' cannot be null or empty.", nameof(id));
                }

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }

                var productos = await this.productoApplication.DeleteById(id);
                return new OkObjectResult(productos);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al eliminar el productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("ProductInventory")]
        [OpenApiOperation(operationId: "ProductInventory", tags: new[] { "Productos/Codificacion/ProductInventory" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]

        public async Task<IActionResult> ProductInventory(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Productos/Codificacion/ProductInventory")] HttpRequest req,
        ILogger log)
        {

            try
            {
                log.LogInformation($"Product:ProductInventory Inicia obtener todos los productos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var productoInventarioDtos = JsonConvert.DeserializeObject<ProductoInventarioDto[]>(requestBody);
                var response = await this.productoApplication.AddProductoInventario(productoInventarioDtos);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al actualizar inventario producto Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("ProductPrices")]
        [OpenApiOperation(operationId: "ProductPrices", tags: new[] { "Productos/Codificacion/ProductPrices" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]

        public async Task<IActionResult> ProductPrices(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Productos/Codificacion/ProductPrices")] HttpRequest req,
        ILogger log)
        {
            try
            {
                log.LogInformation($"Product:ProductPrices Inicia agregar precio a los productos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var productoPrecios = JsonConvert.DeserializeObject<ProductoPreciosDto[]>(requestBody);
                var response = await productoApplication.AddProductoPrecios(productoPrecios);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Product:ProductPrices fin agregar precio a los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetAndApplyFilters")]
        [OpenApiOperation(operationId: "GetAndApplyFilters", tags: new[] { "Productos/Mk/GetAndApplyFilters" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]
        public async Task<IActionResult> GetAndApplyFilters(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Productos/Mk/GetAndApplyFilters")] HttpRequest req,
           ILogger log)
        {
            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }

                foreach (var header in req.Headers)
                {
                    log.LogInformation($"{header.Key}: {header.Value}");
                }

                string tipoUsuario = string.Empty;
                if (req.Headers.TryGetValue("Type", out StringValues tuEncabezadoValues))
                {
                    tipoUsuario = tuEncabezadoValues.FirstOrDefault();
                }
                log.LogInformation($"Product:ObtenerFiltros Inicia obtener todos los filtros. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var generalFiltersWithResponses = JsonConvert.DeserializeObject<GeneralFiltersWithResponseDto>(requestBody);
                generalFiltersWithResponses.ApplyFiltro.TipoUsuario = tipoUsuario;
                var filtros = await productoApplication.GetAndApplyFilters(generalFiltersWithResponses);
                log.LogInformation($"Product:ObtenerFiltros finaliza obtener todos los filtros sin errores. Fecha:{DateTime.UtcNow}");
                return new OkObjectResult(filtros);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetByRef")]
        [OpenApiOperation(operationId: "GetByRef", tags: new[] { "Productos/GetByRef" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]
        public async Task<IActionResult> GetByRef(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Productos/GetByRef/{referencia}")] HttpRequest req,
        string referencia,  // <-- Parámetro adicional
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }

                if (string.IsNullOrEmpty(referencia))
                {
                    throw new ArgumentException($"'{nameof(referencia)}' cannot be null or empty.", nameof(referencia));
                }

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }

                var producto = await this.productoApplication.GetByRef(referencia);
                return new OkObjectResult(producto);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("UpdateInventory")]
        [OpenApiOperation(operationId: "UpdateInventory", tags: new[] { "Productos/Codificacion/UpdateInventory" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Actualiza inventario")]

        public async Task<IActionResult> UpdateInventory(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Productos/Codificacion/UpdateInventory")] HttpRequest req,
        ILogger log)
        {
            try
            {
                log.LogInformation($"Product:ProductPrices Inicia agregar precio a los productos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var productoPrecios = JsonConvert.DeserializeObject<ProductoRefence[]>(requestBody);
                var response = await productoApplication.UpdateInventory(productoPrecios);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Product:ProductPrices fin agregar precio a los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetProductByEAN")]
        [OpenApiOperation(operationId: "GetProductByEAN", tags: new[] { "Productos/GetProductByEAN" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]
        public async Task<IActionResult> GetProductByEAN(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Productos/GetProductByEAN/{ean}")] HttpRequest req, ILogger logger, string ean)
        {
            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }

                if (string.IsNullOrEmpty(ean))
                {
                    throw new ArgumentException($"'{nameof(ean)}' cannot be null or empty.", nameof(ean));
                }

                if (logger is null)
                {
                    throw new ArgumentNullException(nameof(logger));
                }

                var producto = await this.productoApplication.GetProductByEAN(ean);
                return new OkObjectResult(producto);
            }
            catch (Exception ex)
            {
                return GetFunctionError(logger, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetProductByProveedor")]
        [OpenApiOperation(operationId: "GetProductByProveedor", tags: new[] { "Productos/GetProductByProveedor" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]
        public async Task<IActionResult> GetProductByProveedor(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Productos/GetProductByProveedor/{nit}")] HttpRequest req,
        string nit,  // <-- Parámetro adicional
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }

                if (string.IsNullOrEmpty(nit))
                {
                    throw new ArgumentException($"'{nameof(nit)}' cannot be null or empty.", nameof(nit));
                }

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }

                var producto = await this.productoApplication.GetProductByProveedor(nit);
                return new OkObjectResult(producto);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("AddBanner")]
        [OpenApiOperation(operationId: "AddBanner", tags: new[] { "Productos/AddBanner" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Guarda los Banners")]
        public async Task<IActionResult> AddBanner(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Productos/AddBanner")] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation($"Product:AddBanner Inicia obtener todos los banners. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Banner>(requestBody);
                await this.productoApplication.AddBanner(data);
                return new OkResult();
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los Banners Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetBannerById")]
        [OpenApiOperation(operationId: "GetBannerById", tags: new[] { "Productos/GetBannerById" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de los banners por usuario")]
        public async Task<IActionResult> GetBannerById(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Productos/GetBannerById")] HttpRequest req,  // <-- Parámetro adicional
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }

                log.LogInformation($"Product:AddBanner Inicia obtener todos los banners. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Banner>(requestBody);
                var response = await this.productoApplication.GetBannerById(data);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetProductByProveedorOrAll")]
        [OpenApiOperation(operationId: "GetProductByProveedor", tags: new[] { "Productos/GetProductByProveedorOrAll" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]
        public async Task<IActionResult> GetProductByProveedorOrAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Productos/GetProductByProveedorOrAll")] HttpRequest req,
        // <-- Parámetro adicional
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }
                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<TipoUsuarioDto[]>(requestBody);
                var producto = await this.productoApplication.GetProductByProveedorOrAll(data);
                return new OkObjectResult(producto);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetProductsForSearchAll")]
        [OpenApiOperation(operationId: "GetProductsForSearchAll", tags: new[] { "Productos/GetProductsForSearchAll" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]
        public async Task<IActionResult> GetProductsForSearchAll(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Productos/GetProductsForSearchAll")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }
                string tipoUsuario = string.Empty;
                if (req.Headers.TryGetValue("Type", out StringValues tuEncabezadoValues))
                {
                    tipoUsuario = tuEncabezadoValues.FirstOrDefault();
                }
                log.LogInformation($"Product:ObtenerFiltros Inicia obtener todos los filtros. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var TipeUser = JsonConvert.DeserializeObject<SearchDto>(requestBody);
                TipeUser.TipoUsuario = tipoUsuario;

                var producto = await this.productoApplication.GetProductsBySearch(TipeUser);

                return new OkObjectResult(producto);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetProductByName")]
        [OpenApiOperation(operationId: "GetProductByName", tags: new[] { "Productos/GetProductByName" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]
        public async Task<IActionResult> GetProductByName(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Productos/GetProductByName/{nombre}")] HttpRequest req, string nombre,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }
                log.LogInformation($"Product:GetProductByName Inicia obtener todos los productos por nombre. Fecha:{DateTime.UtcNow}");
                var producto = await this.productoApplication.GetProductByName(nombre);
                return new OkObjectResult(producto);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("DeleteLeonisaProduct")]
        [OpenApiOperation(operationId: "DeleteLeonisaProduct", tags: new[] { "Productos/DeleteLeonisaProduct" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]
        public async Task<IActionResult> DeleteLeonisaProduct(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Productos/DeleteLeonisaProduct")] HttpRequest req,
        // <-- Parámetro adicional
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }
                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }
;
                var producto = await this.productoApplication.DeleteLeonisaProduct();
                return new OkObjectResult(producto);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("AddBannerEventos")]
        [OpenApiOperation(operationId: "AddBannerEventos", tags: new[] { "Productos/AddBannerEventos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los banners")]
        public async Task<IActionResult> AddBannerEventos(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Productos/AddBannerEventos")] HttpRequest req,
        // <-- Parámetro adicional
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }
                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Banner>(requestBody);
                var banner = await this.productoApplication.AddBannerEventos(data);
                return new OkObjectResult(banner);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los banners Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetBannerByEvent")]
        [OpenApiOperation(operationId: "GetBannerByEvent", tags: new[] { "Productos/GetBannerByEvent" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los banners")]
        public async Task<IActionResult> GetBannerByEvent(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Productos/GetBannerByEvent")] HttpRequest req,
        // <-- Parámetro adicional
        ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }
                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Banner>(requestBody);
                var banner = await this.productoApplication.GetBannerByEvent(data);
                return new OkObjectResult(banner);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los banners Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }
    }
}

