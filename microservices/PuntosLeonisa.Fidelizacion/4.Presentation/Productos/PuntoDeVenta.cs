using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.PuntoDeVenta;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Seguridad.Application.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace PuntosLeonisa.Seguridad.Function
{
    public class PuntoDeVenta
    {
        private readonly IPuntoDeVentaApplication _puntoDeVentaApplication;
        private readonly GenericResponse<PuntoDeVenta> responseError;
        private readonly BadRequestObjectResult PuntoDeVentaApplicationErrorResult;

        public PuntoDeVenta(IPuntoDeVentaApplication _puntoDeVentaApplication)
        {
            this._puntoDeVentaApplication = _puntoDeVentaApplication;
            this.PuntoDeVentaApplicationErrorResult = new BadRequestObjectResult(this.responseError);
            this.responseError = new GenericResponse<PuntoDeVenta>();
        }
        private IActionResult GetFunctionError(ILogger log, string logMessage, Exception ex)
        {
            log.LogError(ex, logMessage, null);
            this.responseError.Message = ex.Message;
            this.responseError.IsSuccess = false;
            return this.PuntoDeVentaApplicationErrorResult;
        }

        [FunctionName("AddPuntoDeVenta")]
        [OpenApiOperation(operationId: "PuntoDeVenta", tags: new[] { "PuntosDeVenta/PuntoDeVenta" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<PuntoDeVentaDto>), Description = "Guarda el Punto de Venta")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                log.LogInformation($"PuntosDeVenta: AddPuntoDeVenta Inicia a añadir el punto de venta. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<PuntoDeVentaDto>(requestBody);
                var result = await this._puntoDeVentaApplication.Add(data);
                return new OkObjectResult(result);


            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los PuntosDeVenta Fecha:" + DateTime.UtcNow.ToString(), ex);
            }

        }

        [FunctionName("GetPuntosDeVenta")]
        [OpenApiOperation(operationId: "GetPuntosDeVenta", tags: new[] { "PuntosDeVenta/GetPuntosDeVenta" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<IEnumerable<PuntoDeVentaDto>>), Description = "Lista de dtos con los PuntosDeVenta")]
        public async Task<IActionResult> GetPuntosDeVenta(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "PuntosDeVenta/GetPuntosDeVenta")] HttpRequest req,
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

                log.LogInformation($"PuntosDeVenta : GetPuntosDeVenta Inicia obtener todos los PuntosDeVenta. Fecha:{DateTime.UtcNow}");
                var puntoDeVenta = await this._puntoDeVentaApplication.GetAll();
                log.LogInformation($"PuntosDeVenta: GetPuntosDeVenta finaliza obtener todos los PuntosDeVenta sin errores. Fecha:{DateTime.UtcNow}");
                return new OkObjectResult(puntoDeVenta);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los PuntosDeVenta Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetPuntoDeVenta")]
        [OpenApiOperation(operationId: "PuntoDeVenta/GetPuntoDeVenta", tags: new[] { "GetPuntoDeVenta" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los PuntoDeVenta")]
        public async Task<IActionResult> GetPuntoDeVenta(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "PuntoDeVenta/GetPuntoDeVenta/{id}")] HttpRequest req,
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

                var puntoDeVenta = await this._puntoDeVentaApplication.GetById(id);

                return new OkObjectResult(puntoDeVenta);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("DeletePuntoDeVenta")]
        [OpenApiOperation(operationId: "DeletePuntoDeVenta", tags: new[] { "DeletePuntoDeVenta" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los puntos de venta")]

        public async Task<IActionResult> DeletePuntoDeVenta(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "PuntosDeVenta/DeletePuntoDeVenta/{id}")] HttpRequest req,
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


                var puntoDeVenta = await this._puntoDeVentaApplication.DeleteById(id);

                return new OkObjectResult(puntoDeVenta);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al eliminar el punto de venta, Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("LoadPuntosVenta")]
        [OpenApiOperation(operationId: "LoadPuntosVenta", tags: new[] { "Usuario/LoadPuntosVenta" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<PuntoDeVentaDto>), Description = "Carga masiva de Puntos de Venta")]
        public async Task<IActionResult> LoadUsuarios(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Usuario/LoadPuntosVenta")] HttpRequest req,
           ILogger log)
        {


            try
            {
                log.LogInformation($"Usuario:LoadUsuarios Inicia agregar usuarios masivos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var PuntoVenta = JsonConvert.DeserializeObject<PuntoDeVentaDto[]>(requestBody);
                var punto = await this._puntoDeVentaApplication.AddRange(PuntoVenta);

                return new OkObjectResult(punto);

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("LoadPuntoVentaVar")]
        [OpenApiOperation(operationId: "LoadPuntoVentaVar", tags: new[] { "PuntosDeVenta/LoadPuntoVentaVar" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<PuntoVentaVarDto>), Description = "Carga masiva de registros de liquidacion")]
        public async Task<IActionResult> LoadPuntoVentaVar(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PuntosDeVenta/LoadPuntoVentaVar")] HttpRequest req,
           ILogger log)
        {


            try
            {
                log.LogInformation($"Usuario:LoadPuntoVentaVar Inicia agregar registros masivos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var PuntoVenta = JsonConvert.DeserializeObject<PuntoVentaVarDto[]>(requestBody);
                var punto = await this._puntoDeVentaApplication.AddPuntoVentaVar(PuntoVenta);

                return new OkObjectResult(punto);

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("LoadAsignacion")]
        [OpenApiOperation(operationId: "LoadAsignacion", tags: new[] { "PuntosDeVenta/LoadAsignacion" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<AsignacionDto>), Description = "Carga masiva de registros de liquidacion")]
        public async Task<IActionResult> LoadAsignacion(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PuntosDeVenta/LoadAsignacion")] HttpRequest req,
           ILogger log)
        {


            try
            {
                log.LogInformation($"PuntosDeVenta:LoadAsignacion Inicia agregar registros masivos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var PuntoVenta = JsonConvert.DeserializeObject<AsignacionDto[]>(requestBody);
                var punto = await this._puntoDeVentaApplication.AddAsignacion(PuntoVenta);

                return new OkObjectResult(punto);

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al guardar los registros:" + DateTime.UtcNow.ToString(), ex);
            }
        }
    }
}

