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
using PuntosLeonisa.Products.Domain.Model;

namespace Usuarios
{
    public static class Usuarios
    {
        [FunctionName("Usuarios")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Usuario>(requestBody);
                var aplication = new UsuariosApplication();

                aplication.GuardarUsuario(data);
                return new OkResult();

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }


        }


        [FunctionName("GetUsuarios")]
        public static async Task<IActionResult> GetUsuarios(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];
            try
            {

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                dynamic data = JsonConvert.DeserializeObject(requestBody);

                var aplication = new UsuariosApplication();

                var usuarios = aplication.GetAll().GetAwaiter().GetResult();

                return new OkObjectResult(new { usuarios = usuarios, status = 200 });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

        [FunctionName("LoadUsers")]
        public static async Task<IActionResult> LoadUsers(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var users = JsonConvert.DeserializeObject<Usuario[]>(requestBody);
                //name = name ?? data?.name;
                var aplication = new UsuariosApplication();

                aplication.LoadUsers(users);

                return new OkObjectResult(new { });

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

        [FunctionName("GetUser")]
        public static async Task<IActionResult> GetUser(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetUser/{id}")] HttpRequest req,
           string id,  // <-- Parámetro adicional
           ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var aplication = new UsuariosApplication();
                var usuarios = await aplication.GetById(id);

                return new OkObjectResult(new { usuarios = usuarios, status = 200 });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }
    }
}
