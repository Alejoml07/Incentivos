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

namespace Productos
{
    public class Productos
    {
        private readonly IProductApplication productoApplication;

        public Productos(IProductApplication productoApplication)
        {
            this.productoApplication = productoApplication;
        }

        [FunctionName("Productos")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
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
                return new BadRequestObjectResult(ex);
            }


        }

        [FunctionName("GetProductos")]
        public  async Task<IActionResult> GetProductos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];
            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);


                var productos = this.productoApplication.GetAll();

                return new OkObjectResult(productos);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
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
        public  async Task<IActionResult> LoadProducts(
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
                return new BadRequestObjectResult(ex);
            }
        }

        [FunctionName("GetProduct")]
        public  async Task<IActionResult> GetProduct(
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
                return new BadRequestObjectResult(ex);
            }
        }

        [FunctionName("DeleteProduct")]
        public  async Task<IActionResult> DeleteProduct(
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
                return new BadRequestObjectResult(ex);
            }
        }
        
    }
}

