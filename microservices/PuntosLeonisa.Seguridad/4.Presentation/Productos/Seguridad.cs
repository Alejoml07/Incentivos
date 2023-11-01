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
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.Net;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Application.Core;

namespace Usuarioos
{
    public class Seguridad
    {
        private readonly IUsuarioApplication usuarioApplication;
        private readonly GenericResponse<UsuarioDto> responseError;
        private readonly BadRequestObjectResult productoApplicationErrorResult;

        public Seguridad(IUsuarioApplication usuarioApplication)
        {
            this.usuarioApplication = usuarioApplication;
            this.responseError = new GenericResponse<UsuarioDto>();
            this.productoApplicationErrorResult = new BadRequestObjectResult(this.responseError);
        }

        [FunctionName("Usuarios")]
        [OpenApiOperation(operationId: "Usuarioos", tags: new[] { "Usuarioos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Guarda el producto")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                log.LogInformation($"Usuario:GetUsuarioos Inicia obtener todos los usuarios. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<UsuarioDto>(requestBody);

                await this.usuarioApplication.Add(data);
                return new OkResult();

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }


        }

        [FunctionName("GetUsuarios")]
        [OpenApiOperation(operationId: "GetUsuarioos", tags: new[] { "GetUsuarioos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los usuarios")]
        public async Task<IActionResult> GetUsuarioos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
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

                log.LogInformation($"Usuario:GetUsuarioos Inicia obtener todos los usuarios. Fecha:{DateTime.UtcNow}");
                var usuarios = await usuarioApplication.GetAll();
                log.LogInformation($"Usuario:GetUsuarioos finaliza obtener todos los usuarios sin errores. Fecha:{DateTime.UtcNow}");
                return new OkObjectResult(usuarios);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        private IActionResult GetFunctionError(ILogger log, string logMessage, Exception ex)
        {
            log.LogError(ex, logMessage, null);
            this.responseError.Message = ex.Message;
            this.responseError.IsSuccess = false;
            return this.productoApplicationErrorResult;
        }

 
        [FunctionName("LoadUsuarios")]
        [OpenApiOperation(operationId: "LoadUsuarios", tags: new[] { "LoadUsuarios" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los usuarios")]
        public async Task<IActionResult> LoadUsuarios(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
           ILogger log)
        {
            

            try
            {
                log.LogInformation($"Usuario:LoadUsuarios Inicia agregar usuarios masivos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var products = JsonConvert.DeserializeObject<UsuarioDto[]>(requestBody);

                await this.usuarioApplication.AddRange(products);

                return new OkResult();

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetUsuario")]
        [OpenApiOperation(operationId: "GetUsuario", tags: new[] { "GetUsuario" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los usuarios")]
        public async Task<IActionResult> GetUsuario(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetUsuario/{id}")] HttpRequest req,
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

                var producto = await this.usuarioApplication.GetById(id);

                return new OkObjectResult(producto);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("DeleteUsuario")]
        [OpenApiOperation(operationId: "DeleteUsuario", tags: new[] { "DeleteUsuario" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los usuarios")]

        public async Task<IActionResult> DeleteUsuario(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteUsuario/{id}")] HttpRequest req,
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


                var usuarios = await this.usuarioApplication.DeleteById(id);

                return new OkObjectResult(usuarios);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al eliminar el usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }


    }
}

