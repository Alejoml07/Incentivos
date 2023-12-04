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
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using System.Collections.Generic;
using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.WishList;

namespace Usuarioos
{
    public class Fidelizacion
    {
        private readonly IFidelizacionApplication puntosApplication;
        private readonly GenericResponse<PuntosManualDto> responseError;
        private readonly GenericResponse<WishListDto> responseError2;
        private readonly BadRequestObjectResult puntosApplicationErrorResult;

        public Fidelizacion(IFidelizacionApplication usuarioApplication)
        {
            puntosApplication = usuarioApplication;
            this.responseError = new GenericResponse<PuntosManualDto>();
            this.responseError2 = new GenericResponse<WishListDto>();
            this.puntosApplicationErrorResult = new BadRequestObjectResult(this.responseError);
        }

        [FunctionName("Puntos")]
        [OpenApiOperation(operationId: "Puntos", tags: new[] { "Puntos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Guarda el producto")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                log.LogInformation($"Puntos:GetPuntos Inicia obtener todos los Puntos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<PuntosManualDto>(requestBody);

                await this.puntosApplication.Add(data);
                return new OkResult();

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }


        }

        [FunctionName("GetPuntos")]
        [OpenApiOperation(operationId: "GetPuntos", tags: new[] { "GetPuntos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los usuarios")]
        public async Task<IActionResult> GetPuntos(
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

                log.LogInformation($"Puntos:GetPuntos Inicia obtener todos los puntos. Fecha:{DateTime.UtcNow}");
                var puntos = await puntosApplication.GetAll();
                log.LogInformation($"Puntos:GetPuntos finaliza obtener todos los puntos sin errores. Fecha:{DateTime.UtcNow}");
                return new OkObjectResult(puntos);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los puntos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        private IActionResult GetFunctionError(ILogger log, string logMessage, Exception ex)
        {
            log.LogError(ex, logMessage, null);
            this.responseError.Message = ex.Message;
            this.responseError.IsSuccess = false;
            return this.puntosApplicationErrorResult;
        }


        [FunctionName("LoadPuntos")]
        [OpenApiOperation(operationId: "LoadPuntos", tags: new[] { "LoadPuntos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los usuarios")]
        public async Task<IActionResult> LoadPuntos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
           ILogger log)
        {


            try
            {
                log.LogInformation($"Puntos:LoadPuntos Inicia agregar puntos masivos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var points = JsonConvert.DeserializeObject<PuntosManualDto[]>(requestBody);

                await this.puntosApplication.AddRange(points);

                return new OkResult();

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los puntos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetPunto")]
        [OpenApiOperation(operationId: "GetPunto", tags: new[] { "GetPunto" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los usuarios")]
        public async Task<IActionResult> GetPunto(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetPunto/{id}")] HttpRequest req,
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

                var producto = await this.puntosApplication.GetById(id);

                return new OkObjectResult(producto);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los puntos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("DeletePunto")]
        [OpenApiOperation(operationId: "DeletePunto", tags: new[] { "DeletePunto" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los usuarios")]

        public async Task<IActionResult> DeletePunto(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeletePunto/{id}")] HttpRequest req,
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


                var puntos = await this.puntosApplication.DeleteById(id);

                return new OkObjectResult(puntos);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al eliminar los puntos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("DeletePuntos")]
        [OpenApiOperation(operationId: "DeletePuntos", tags: new[] { "DeletePuntos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los usuarios")]

        public async Task<IActionResult> DeletePuntos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DeletePuntos")] HttpRequest req,
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
                var puntos = JsonConvert.DeserializeObject<string[]>(requestBody);
                var response = new GenericResponse<IList<PuntosManualDto>>();
                response.Result = new List<PuntosManualDto>();
                foreach (var punto in puntos)
                {
                    response.Result.Add(this.puntosApplication.DeleteById(punto).GetAwaiter().GetResult().Result);
                }

                return new OkObjectResult(response);

            }

            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al eliminar los puntos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("WishList")]
        [OpenApiOperation(operationId: "WishList", tags: new[] { "WishList" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Guarda la wishlist")]
        public async Task<IActionResult> WishList(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                log.LogInformation($"WishList : WishList Inicia obtener todos los productos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<WishListDto>(requestBody);

                var result = await this.puntosApplication.WishListAdd(data);
                return new OkObjectResult(result);

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener el user y product, Fecha:" + DateTime.UtcNow.ToString(), ex);
            }

        }

        // delete wishlist
        [FunctionName("DeleteWishList")]
        [OpenApiOperation(operationId: "DeleteWishList", tags: new[] { "DeleteWishList" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Guarda la wishlist")]
        public async Task<IActionResult> DeleteWishList(
                       [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "fidelizacion/DeleteWishList/{id}")] HttpRequest req,
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

                await this.puntosApplication.WishListDeleteById(id);
                return new OkResult();
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al eliminar los puntos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

    }
}

