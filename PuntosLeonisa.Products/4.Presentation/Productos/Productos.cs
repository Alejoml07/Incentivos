using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuntosLeonisa.Products.Application;
using PuntosLeonisa.Products.Domain;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Productos
{
    public static class Productos
    {
        [FunctionName("Productos")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Producto>(requestBody);
                data.Id = Guid.NewGuid().ToString();
                var aplication = new ProductosApplication();

                aplication.GuardarProducto(data);
                return new OkResult();

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }


        }

        [FunctionName("GetProductos")]
        public static async Task<IActionResult> GetProductos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];
            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                var aplication = new ProductosApplication();

                var productos = aplication.GetAll().GetAwaiter().GetResult();

                return new OkObjectResult(new { productos = productos, status = 200 });
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
        public static async Task<IActionResult> LoadProducts(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var products = JsonConvert.DeserializeObject<Producto[]>(requestBody);
                //name = name ?? data?.name;
                var aplication = new ProductosApplication();

                aplication.LoadProducts(products);

                return new OkObjectResult(new { });

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

        [FunctionName("GetProduct")]
        public static async Task<IActionResult> GetProduct(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetProduct/{id}")] HttpRequest req,
           string id,  // <-- Parámetro adicional
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var aplication = new ProductosApplication();
                var productos = aplication.GetById(id).GetAwaiter().GetResult();

                return new OkObjectResult(new { productos = productos, status = 200 });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

    }
}

