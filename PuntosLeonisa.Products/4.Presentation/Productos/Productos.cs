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
using PuntosLeonisa.Products.Domain.Service.DTO;
using PuntosLeonisa.Products.Infrasctructure.Common.Communication;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.Net;

namespace Productos
{
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
        [OpenApiOperation(operationId: "Productos", tags: new[] { "Productos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Guarda el producto")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
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
        [OpenApiOperation(operationId: "GetProductos", tags: new[] { "GetProductos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]
        public async Task<IActionResult> GetProductos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
           ILogger log)
        {

            try
            {

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

            var formdata = await req.ReadFormAsync();
            var file = req.Form.Files["image"];

            if (file == null || file.Length == 0)
            {
                return new BadRequestObjectResult("File missing");
            }

            string connectionString = "DefaultEndpointsProtocol=https;AccountName=stgactincentivos;AccountKey=mtBoBaUJu8BKcHuCfdWzk1au7Upgif0rlzD+BlfAJZBsvQ02CiGzCNG5gj1li10GF8RpUwz6h+Mj+AStMOwyTA==;EndpointSuffix=core.windows.net";
            string containerName = "$web";
            string blobName = "/img/" + Path.GetRandomFileName() + Path.GetExtension(file.FileName);

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            string mimeType = file.ContentType;

            BlobUploadOptions uploadOptions = new BlobUploadOptions
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
        [OpenApiOperation(operationId: "LoadProducts", tags: new[] { "LoadProducts" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]
        public async Task<IActionResult> LoadProducts(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var products = JsonConvert.DeserializeObject<ProductoDto[]>(requestBody);

                await this.productoApplication.AddRange(products);

                return new OkResult();

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetProduct")]
        [OpenApiOperation(operationId: "GetProduct", tags: new[] { "GetProduct" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]
        public async Task<IActionResult> GetProduct(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetProduct/{id}")] HttpRequest req,
           string id,  // <-- Parámetro adicional
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {

                var producto = await this.productoApplication.GetById(id);

                return new OkObjectResult(producto);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("DeleteProduct")]
        [OpenApiOperation(operationId: "DeleteProduct", tags: new[] { "DeleteProduct" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los productos")]

        public async Task<IActionResult> DeleteProduct(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteProduct/{id}")] HttpRequest req,
           string id,  // <-- Parámetro adicional
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var productos = await this.productoApplication.DeleteById(id);

                return new OkObjectResult(productos);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al eliminar el productos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

    }
}

