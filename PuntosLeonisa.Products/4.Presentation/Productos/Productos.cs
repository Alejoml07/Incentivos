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

            //string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Producto>(requestBody);
            //name = name ?? data?.name;
            data.Id = Guid.NewGuid().ToString();
            var aplication = new ProductosApplication();

            aplication.GuardarProducto(data);

            return new OkObjectResult(new { });
        }

        [FunctionName("GetProductos")]
        public static async Task<IActionResult> GetProductos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            //name = name ?? data?.name;

            var aplication = new ProductosApplication();

            var productos = aplication.GetAll().GetAwaiter().GetResult();

            return new OkObjectResult(new { productos = productos, status = 200 });
        }

        [FunctionName("LoadProducts")]
        public static async Task<IActionResult> LoadProducts(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];
            try { 
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var products = JsonConvert.DeserializeObject <Producto[]>(requestBody);
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
    }
}

