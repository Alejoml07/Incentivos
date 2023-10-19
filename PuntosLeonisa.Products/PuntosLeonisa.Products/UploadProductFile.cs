using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuntosLeonisa.Products.Domain;
using Microsoft.EntityFrameworkCore;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;


using (var productos = new CosmoDB())
{
    var producto1 = new Productos
    #region Inserting Products
    {
        Id = Guid.NewGuid().ToString(),
        Referencia = "Pl123",
        Nombre = "Planca",
        Video = "Melo",
        Caracteristicas = "Melisimo",
        Descripcion = "Calienta melo",
        Puntos = 1042,
        TiempoEntrega = "1 mes",
        Estado = 1,
        Fecha = new DateTime(),
        ImagenPrincipal = "Ya por favor",
        Imagen1 = "Para que",
        Imagen2 = "Tantas",
        Imagen3 = "Imagenes",
        Proveedor = "Rappi",
        Correo = "Yaporfavor@gmail.com",
        TipoPremio = 1,
        Actualizado = 2,
        UrlImagen = "Imagen.png"


    };

    productos.Productos.Add(producto1);

    await productos.SaveChangesAsync();
    #endregion
}


namespace PuntosLeonisa.Products

{
    public static class UploadProductFile
    {


        [FunctionName("UploadProductFile")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }



    }



    
}

