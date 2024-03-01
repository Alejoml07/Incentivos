using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.Net;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Application.Core;
using System.Collections.Generic;

namespace Usuarios
{
    public class Proveedor
    {
        private readonly IProveedorApplication proveedorApplication;
        private readonly GenericResponse<ProveedorDto> responseError;
        private readonly BadRequestObjectResult productoApplicationErrorResult;

        public Proveedor(IProveedorApplication proveedorApplication)
        {
            this.proveedorApplication = proveedorApplication;
            this.responseError = new GenericResponse<ProveedorDto>();
            this.productoApplicationErrorResult = new BadRequestObjectResult(this.responseError);
        }

        [FunctionName("Proveedor")]
        [OpenApiOperation(operationId: "Proveedor", tags: new[] { "Proveedor/Proveedor" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<UsuarioDto>), Description = "Guarda el producto")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                log.LogInformation($"Proveedor:GetProveedor Inicia obtener todos los Proveedores. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<ProveedorDto>(requestBody);

                await this.proveedorApplication.Add(data);
                return new OkResult();

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetProveedores")]
        [OpenApiOperation(operationId: "GetProveedores", tags: new[] { "Proveedor/GetProveedores" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<IEnumerable<UsuarioDto>>), Description = "Lista de dtos con los usuarios")]
        public async Task<IActionResult> GetProveedores(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Proveedor/GetProveedores")] HttpRequest req,
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

                log.LogInformation($"Proveedor:GetProveedores Inicia obtener todos los Proveedores. Fecha:{DateTime.UtcNow}");
                var proveedores = await proveedorApplication.GetAll();
                log.LogInformation($"Proveedor:GetProveedores finaliza obtener todos los proveedores sin errores. Fecha:{DateTime.UtcNow}");
                return new OkObjectResult(proveedores);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los proveedores Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        private IActionResult GetFunctionError(ILogger log, string logMessage, Exception ex)
        {
            log.LogError(ex, logMessage, null);
            this.responseError.Message = ex.Message;
            this.responseError.IsSuccess = false;
            return this.productoApplicationErrorResult;
        }

        [FunctionName("GetProveedor")]
        [OpenApiOperation(operationId: "Proveedor/GetProveedor", tags: new[] { "GetProveedor" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los usuarios")]
        public async Task<IActionResult> GetProveedor(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Proveedor/GetProveedor/{Nit}")] HttpRequest req,
           string Nit,  // <-- Parámetro adicional
           ILogger log)
        {


            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }

                if (string.IsNullOrEmpty(Nit))
                {
                    throw new ArgumentException($"'{nameof(Nit)}' cannot be null or empty.", nameof(Nit));
                }

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }

                var producto = await this.proveedorApplication.GetById(Nit);

                return new OkObjectResult(producto);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener el proveedor Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("DeleteProveedor")]
        [OpenApiOperation(operationId: "DeleteProveedor", tags: new[] { "DeleteProveedor" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con los usuarios")]

        public async Task<IActionResult> DeleteProveedor(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Proveedor/DeleteProveedor/{Nit}")] HttpRequest req,
           string Nit,  // <-- Parámetro adicional
           ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }

                if (string.IsNullOrEmpty(Nit))
                {
                    throw new ArgumentException($"'{nameof(Nit)}' cannot be null or empty.", nameof(Nit));
                }

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }


                var proveedor = await this.proveedorApplication.DeleteById(Nit);

                return new OkObjectResult(proveedor);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al eliminar el proveedor Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("UpdateProveedor")]
        [OpenApiOperation(operationId: "UpdateProveedor", tags: new[] { "Proveedor/UpdateProveedor" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<ProveedorDto>), Description = "Hace Update del proveedor")]
        public async Task<IActionResult> UpdateProveedor(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Proveedor/UpdateProveedor")] HttpRequest req,
            ILogger log)
        {

            try
            {
                log.LogInformation($"Proveedor : UpdateProveedor Inicia a actualizar todos los Proveedores. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<ProveedorDto>(requestBody);

                var response = await this.proveedorApplication.Update(data);
                return new OkObjectResult(response);

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al actualizar los proveedores Fecha:" + DateTime.UtcNow.ToString(), ex);
            }


        }
    }
}

