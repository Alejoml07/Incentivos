﻿using Microsoft.AspNetCore.Http;
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
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

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
        public IActionResult GetFunctionError(ILogger log, string message, Exception ex)
        {
            log.LogError(ex, message);
            this.responseError.Message = message;
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

        //[FunctionName("LiquidacionPuntosMes")]
        //[OpenApiOperation(operationId: "LiquidacionPuntosMes", tags: new[] { "PuntosDeVenta/LiquidacionPuntosMes" })]
        //[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<LiquidacionPuntos>), Description = "Carga masiva de registros de liquidacion")]
        //public async Task<IActionResult> LiquidacionPuntosMes(
        //   [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PuntosDeVenta/LiquidacionPuntosMes")] HttpRequest req,
        //   ILogger log)
        //{


        //    try
        //    {
        //        log.LogInformation($"PuntosDeVenta:LoadAsignacion Inicia agregar registros masivos. Fecha:{DateTime.UtcNow}");
        //        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        //        var PuntoVenta = JsonConvert.DeserializeObject<LiquidacionPuntos>(requestBody);
        //        var punto = await this._puntoDeVentaApplication.LiquidacionPuntosMes(PuntoVenta);
        //        return new OkObjectResult(punto);

        //    }
        //    catch (Exception ex)
        //    {
        //        //log.LogError(ex, "Error al guardar los registros:" + DateTime.UtcNow.ToString());
        //        return GetFunctionError(log, "Error al guardar los registros:" + DateTime.UtcNow.ToString(), ex);
        //    }
        //}




        [FunctionName("AddAndDeleteVentaVarAndHistoria")]
        [OpenApiOperation(operationId: "AddAndDeleteVentaVarAndHistoria", tags: new[] { "PuntosDeVenta/AddAndDeleteVentaVarAndHistoria" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<LiquidacionPuntos>), Description = "Carga masiva de registros de liquidacion")]
        public async Task<IActionResult> AddAndDeleteVentaVarAndHistoria(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PuntosDeVenta/AddAndDeleteVentaVarAndHistoria")] HttpRequest req,
           ILogger log)
        {


            try
            {
                log.LogInformation($"PuntosDeVenta:LoadAsignacion Inicia agregar registros masivos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var PuntoVenta = JsonConvert.DeserializeObject<LiquidacionPuntos>(requestBody);
                var punto = await this._puntoDeVentaApplication.AddAndDeleteVentaVarAndHistoria(PuntoVenta);
                return new OkObjectResult(punto);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al guardar los registros:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("LoadPuntoVentaHistoria")]
        [OpenApiOperation(operationId: "LoadPuntoVentaHistoria", tags: new[] { "PuntosDeVenta/LoadPuntoVentaHistoria" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<PuntoVentaHistoriaDto>), Description = "Carga masiva de registros de liquidacion")]
        public async Task<IActionResult> LoadPuntoVentaHistoria(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PuntosDeVenta/LoadPuntoVentaHistoria")] HttpRequest req,
           ILogger log)
        {
            try
            {
                log.LogInformation($"Usuario:LoadPuntoVentaHistoria Inicia agregar registros masivos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var PuntoVenta = JsonConvert.DeserializeObject<PuntoVentaHistoriaDto[]>(requestBody);
                var punto = await this._puntoDeVentaApplication.AddPuntoVentaHistoria(PuntoVenta);

                return new OkObjectResult(punto);

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetInfoWithSpace")]
        [OpenApiOperation(operationId: "GetInfoWithSpace", tags: new[] { "PuntosDeVenta/GetInfoWithSpace" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(IEnumerable<PuntoVentaHistoriaDto>), Description = "Carga masiva de registros de liquidacion")]
        public async Task<IActionResult> GetInfoWithSpace(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "PuntosDeVenta/GetInfoWithSpace")] HttpRequest req,
           ILogger log)
        {
            try
            {
                log.LogInformation($"Usuario:LoadPuntoVentaHistoria Inicia agregar registros masivos. Fecha:{DateTime.UtcNow}");
                var punto = await this._puntoDeVentaApplication.GetInfoWithSpace();
                return new OkObjectResult(punto);

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("OrquestacionLiquidacionPuntosMes")]
        public static async Task<IActionResult> OrquestacionLiquidacionPuntosMes(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            try
            {
                // Reemplaza "LiquidacionPuntos" con el tipo de objeto adecuado
                var puntoVenta = context.GetInput<LiquidacionPuntos>();
                var resultado = await context.CallActivityAsync<IActionResult>("LiquidacionPuntosMes", puntoVenta);
                return resultado;
            }
            catch (Exception ex)
            {
                // Manejo de errores
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [FunctionName("LiquidacionPuntosMes")]
        public async Task<IActionResult> LiquidacionPuntosMes(
        [ActivityTrigger] LiquidacionPuntos puntoVenta,
        ILogger log)
        {
            try
            {
                log.LogInformation($"PuntosDeVenta:LoadAsignacion Inicia agregar registros masivos. Fecha:{DateTime.UtcNow}");
                // No es necesario deserializar el cuerpo de la solicitud, puntoVenta ya es el objeto deserializado
                var resultado = await this._puntoDeVentaApplication.LiquidacionPuntosMes(puntoVenta);
                return new OkObjectResult(resultado);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al guardar los registros:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("IniciarLiquidacionPuntosMes")]
        public static async Task<IActionResult> IniciarLiquidacionPuntosMes(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PuntosDeVenta/LiquidacionPuntosMes")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var puntoVenta = JsonConvert.DeserializeObject<LiquidacionPuntos>(requestBody);

            // Start the orchestration manually
            string instanceId = await starter.StartNewAsync("OrquestacionLiquidacionPuntosMes", puntoVenta);

            log.LogInformation($"Orquestación de liquidación de puntos iniciada con ID: {instanceId}.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("ValidarLiquidacion")]
        [OpenApiOperation(operationId: "ValidarLiquidacion", tags: new[] { "PuntosDeVenta/ValidarLiquidacion" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Carga masiva de registros de liquidacion")]
        public async Task<IActionResult> ValidarLiquidacion(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "PuntosDeVenta/ValidarLiquidacion")] HttpRequest req,
           ILogger log)
        {
            try
            {
                var punto = await this._puntoDeVentaApplication.ValidarLiquidacion();
                return new OkObjectResult(punto);

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetSeguimientoLiquidacion")]
        [OpenApiOperation(operationId: "GetSeguimientoLiquidacion", tags: new[] { "PuntosDeVenta/GetSeguimientoLiquidacion" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Get carga masiva de registros de liquidacion")]
        public async Task<IActionResult> GetSeguimientoLiquidacion(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PuntosDeVenta/GetSeguimientoLiquidacion")] HttpRequest req,
           ILogger log)
        {
            try
            {
                log.LogInformation($"Reportes:GetSeguimientoLiquidacion Inicia obtener registros de liquidacion. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var fechas = JsonConvert.DeserializeObject<Fechas>(requestBody);
                var punto = await this._puntoDeVentaApplication.GetSeguimientoLiquidacion(fechas);
                return new OkObjectResult(punto);

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("EliminarExcels")]
        [OpenApiOperation(operationId: "EliminarExcels", tags: new[] { "PuntosDeVenta/EliminarExcels" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Get carga masiva de registros de liquidacion")]
        public async Task<IActionResult> EliminarExcels(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "PuntosDeVenta/EliminarExcels")] HttpRequest req,
           ILogger log)
        {
            try
            {
                log.LogInformation($"Reportes:GetSeguimientoLiquidacion Inicia obtener registros de liquidacion. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var fechas = JsonConvert.DeserializeObject<LiquidacionPuntos>(requestBody);
                var punto = await this._puntoDeVentaApplication.EliminarExcels(fechas);
                return new OkObjectResult(punto);

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }


        [FunctionName("ActualizarCarritoLideres")]
        [OpenApiOperation(operationId: "ActualizarCarritoLideres", tags: new[] { "ActualizarCarritoLideres" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Datos enviados correctamente")]
        public async Task<IActionResult> ActualizarCarritoLideres(
       [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fidelizacion/ActualizarCarritoLideres")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {

                var response = await this._puntoDeVentaApplication.ActualizarCarritoLideres();
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los datos:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("ActualizarPuntosLideres")]
        [OpenApiOperation(operationId: "ActualizarCarritoLideres", tags: new[] { "ActualizarPuntosLideres" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Datos enviados correctamente")]
        public async Task<IActionResult> ActualizarPuntosLideres(
       [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fidelizacion/ActualizarPuntosLideres")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {

                var response = await this._puntoDeVentaApplication.ActualizarPuntosLideres();
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los datos:" + DateTime.UtcNow.ToString(), ex);
            }
        }
    }
}

