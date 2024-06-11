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
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using System.Collections.Generic;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.WishList;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Carrito;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.MovimientoPuntos;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.FidelizacionPuntos;
using Microsoft.Azure.ServiceBus;
using System.Linq;
using System.Text;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Variables;
using PuntosLeonisa.Fidelizacion.Application.Core.Interfaces;

namespace Usuarioos
{
    public class Fidelizacion
    {
        private readonly IFidelizacionApplication puntosApplication;
        private readonly GenericResponse<PuntosManualDto> responseError;
        private readonly BadRequestObjectResult puntosApplicationErrorResult;

        public Fidelizacion(IFidelizacionApplication usuarioApplication)
        {
            puntosApplication = usuarioApplication;
            this.responseError = new GenericResponse<PuntosManualDto>();
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
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de puntos")]
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

                var puntos = await this.puntosApplication.AddRange(points);
                return new OkObjectResult(puntos);

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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/WishList/Create")] HttpRequest req,
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

        //Get wishlist by user
        [FunctionName("GetWishListByUser")]
        [OpenApiOperation(operationId: "GetWishListByUser", tags: new[] { "GetWishListByUser" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Guarda la wishlist")]
        public async Task<IActionResult> GetWishListByUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fidelizacion/GetWishListByUser")] HttpRequest req,
            ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {

                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }

                var id = req.Headers["em"].ToString();

                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException($"'{nameof(id)}' cannot be null or empty.", nameof(id));
                }

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }

                var result = await this.puntosApplication.WishListGetByUser(id);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al eliminar los puntos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("Carrito")]
        [OpenApiOperation(operationId: "Carrito", tags: new[] { "Carrito" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Guarda el Carrito")]
        public async Task<IActionResult> Carrito(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/Carrito/Create")] HttpRequest req,
            ILogger log)
        {

            try
            {
                log.LogInformation($"Carrito : Carrito Inicia obtener todos los productos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Carrito>(requestBody);

                var result = await this.puntosApplication.CarritoAdd(data);
                return new OkObjectResult(result);

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener el user y product, Fecha:" + DateTime.UtcNow.ToString(), ex);
            }

        }

        [FunctionName("DeleteCarrito")]
        [OpenApiOperation(operationId: "DeleteCarrito", tags: new[] { "DeleteCarrito" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Guarda el carrito")]
        public async Task<IActionResult> DeleteCarrito(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "fidelizacion/DeleteCarrito/{id}")] HttpRequest req,
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

                var result = await this.puntosApplication.CarritoDeleteById(id);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al eliminar el carrito:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetCarritoByUser")]
        [OpenApiOperation(operationId: "GetCarritoByUser", tags: new[] { "GetCarritoByUser" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Guarda el carrito")]
        public async Task<IActionResult> GetCarritoByUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fidelizacion/GetCarritoByUser")] HttpRequest req,
        ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {

                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }

                var id = req.Headers["em"].ToString();

                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException($"'{nameof(id)}' cannot be null or empty.", nameof(id));
                }

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }

                var result = await this.puntosApplication.CarritoGetByUser(id);
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al eliminar el carrito Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetUsuarioInfoPuntosById")]
        [OpenApiOperation(operationId: "GetUsuarioInfoPuntosById", tags: new[] { "GetUsuarioInfoPuntosById" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los puntos por Id")]
        public async Task<IActionResult> GetUsuarioInfoPuntosById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fidelizacion/GetUsuarioInfoPuntosById/{id}")] HttpRequest req,
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

                var response = await this.puntosApplication.GetUsuarioInfoPuntosById(id);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los puntos:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetUsuarioInfoPuntosAll")]
        [OpenApiOperation(operationId: "GetUsuarioInfoPuntosAll", tags: new[] { "GetUsuarioInfoPuntosAll" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los puntos")]
        public async Task<IActionResult> GetUsuarioInfoPuntosAll(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fidelizacion/GetUsuarioInfoPuntosAll")] HttpRequest req, ILogger log)
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

                var response = await this.puntosApplication.GetUsuarioInfoPuntosAll();
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los puntos:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetUsuariosRedencionPuntos")]
        [OpenApiOperation(operationId: "GetUsuariosRedencionPuntos", tags: new[] { "GetUsuariosRedencionPuntos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene las redenciones")]
        public async Task<IActionResult> GetUsuariosRedencionPuntos(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fidelizacion/GetUsuariosRedencionPuntos")] HttpRequest req, ILogger log)
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

                var response = await this.puntosApplication.GetUsuariosRedencionPuntos();
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener las redenciones:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetUsuariosRedencionPuntosByProveedor")]
        [OpenApiOperation(operationId: "GetUsuariosRedencionPuntosByProveedor", tags: new[] { "GetUsuariosRedencionPuntosByProveedor" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene la redencion por id")]
        public async Task<IActionResult> GetUsuariosRedencionPuntosById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fidelizacion/GetUsuariosRedencionPuntosByProveedor/{id}")] HttpRequest req, ILogger log, string id)
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

                var response = await this.puntosApplication.GetUsuariosRedencionPuntosByProveedor(id);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener las redenciones:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetUsuariosRedencionPuntosByEmail")]
        [OpenApiOperation(operationId: "GetUsuariosRedencionPuntosByEmail", tags: new[] { "GetUsuariosRedencionPuntosByEmail" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene la redencion por Email")]
        public async Task<IActionResult> GetUsuariosRedencionPuntosByEmail(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fidelizacion/GetUsuariosRedencionPuntosByEmail/{email}")] HttpRequest req, ILogger log, string email)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }
                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException($"'{nameof(email)}' cannot be null or empty.", nameof(email));
                }

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }

                var response = await this.puntosApplication.GetUsuariosRedencionPuntosByEmail(email);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener las redenciones:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("AddNroGuiaYTransportadora")]
        [OpenApiOperation(operationId: "GetUsuariosRedencionPuntosByEmail", tags: new[] { "AddNroGuiaYTransportadora" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "hace post de la guia y transportadora")]
        public async Task<IActionResult> AddNroGuiaYTransportadora(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/AddNroGuiaYTransportadora")] HttpRequest req, ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<AddNroGuiaYTransportadora>(requestBody);
                var response = await this.puntosApplication.AddNroGuiaYTransportadora(data);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener las redenciones:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("DevolucionPuntosYCancelarEstado")]
        [OpenApiOperation(operationId: "DevolucionPuntosYCancelarEstado", tags: new[] { "DevolucionPuntosYCancelarEstado" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Devuelve los puntos redimidos al cancelarlos")]
        public async Task<IActionResult> DevolucionPuntosYCancelarEstado(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/DevolucionPuntosYCancelarEstado")] HttpRequest req, ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<DevolucionPuntosDto>(requestBody);
                var response = await this.puntosApplication.DevolucionPuntosYCancelarEstado(data);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener las redenciones:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("Extracto")]
        [OpenApiOperation(operationId: "Extracto", tags: new[] { "Extracto" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Guarda el extracto")]
        public async Task<IActionResult> Extractos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                log.LogInformation($"Extracto :Extracto Inicia obtener todos los Extractos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Extractos>(requestBody);
                await this.puntosApplication.AddExtracto(data);
                return new OkResult();

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetExtractos")]
        [OpenApiOperation(operationId: "GetExtractos", tags: new[] { "GetExtractos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de extractos")]
        public async Task<IActionResult> GetExtractos(
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

                log.LogInformation($"Extractos :GetExtractos Inicia obtener todos los extractos. Fecha:{DateTime.UtcNow}");
                var puntos = await puntosApplication.GetExtractos();
                log.LogInformation($"Extractos :GetExtractos termina obtener todos los extractos. Fecha:{DateTime.UtcNow}");
                return new OkObjectResult(puntos);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los puntos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("LoadExtractos")]
        [OpenApiOperation(operationId: "LoadExtractos", tags: new[] { "LoadExtractos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "agrega masivamente los extractos")]
        public async Task<IActionResult> LoadExtractos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
           ILogger log)
        {


            try
            {
                log.LogInformation($"Extractos:LoadExtractos Inicia agregar extractos masivos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var extractos = JsonConvert.DeserializeObject<Extractos[]>(requestBody);
                var ext = await this.puntosApplication.AddExtractos(extractos);
                return new OkObjectResult(ext);

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los puntos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetExtractosByUsuario")]
        [OpenApiOperation(operationId: "GetExtractosByUsuario", tags: new[] { "GetUsuariosRedencionPuntosByEmail" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los extractos por Cedula")]
        public async Task<IActionResult> GetExtractosByUsuario(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetExtractosByUsuario/{cedula}")] HttpRequest req, ILogger log, string cedula)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }
                if (string.IsNullOrEmpty(cedula))
                {
                    throw new ArgumentException($"'{nameof(cedula)}' cannot be null or empty.", nameof(cedula));
                }

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }

                var response = await this.puntosApplication.GetExtractosByUsuario(cedula);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener las redenciones:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("SincronizarLiquidacionPuntos")]
        [OpenApiOperation(operationId: "GetUsuariosRedencionPuntosByEmail", tags: new[] { "SincronizarLiquidacionPuntos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "hace post de la guia y transportadora")]
        public async Task<IActionResult> SincronizarLiquidacionPuntos(
  [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/SincronizarLiquidacionPuntos")] HttpRequest req, ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {

                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<LiquidacionPuntosDto[]>(requestBody);
                //this.puntosApplication.GuardarLiquidacionPuntos(data);

                string connectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");
                string queueName = "queueliquidacionpuntos";

                var serviceBusClient = new QueueClient(connectionString, queueName);
                // agrupar datos por cedula y mandar un mensaje con la agrupacion de cada cedula y convertir en un array de array con las cedulas
                var agrupado = data.OrderBy(x => x.Cedula).GroupBy(x => x.Cedula).Select(x => x.ToList()).ToList();
                foreach (var item in agrupado)
                {
                    //Delay de 1000 milisegundos
                    //await Task.Delay(50);
                    var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item)));
                    await serviceBusClient.SendAsync(message);
                }
                return new OkResult();
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener las redenciones:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("ProcessMessageFunctionLiquidacion")]
        public void ProcessMessageFunction(
        [ServiceBusTrigger("queueliquidacionpuntos", Connection = "ServiceBusConnectionString")] string myQueueItem,
        ILogger log)
        {
            try
            {

                log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

                var data = JsonConvert.DeserializeObject<List<LiquidacionPuntosDto>>(myQueueItem);
                //aqui obtienes el usuarioinfopuntos y le restas los puntos

                this.puntosApplication.GuardarLiquidacionPuntos(data);



                //this.puntosApplication.GuardarLiquidacionPunto(data);
                log.LogInformation("Mensaje procesado correctamente");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error procesando la redencion");
            }

        }


        [FunctionName("GetReporteRedencion")]
        [OpenApiOperation(operationId: "GetReporteRedencion", tags: new[] { "GetReporteRedencion" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los reportes por fechas")]
        public async Task<IActionResult> GetReporteRedencion(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/GetReporteRedencion")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<ReporteDto>(requestBody);
                var response = await this.puntosApplication.GetReporteRedencion(data);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los reportes:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("CambiarEstadoYLiquidarPuntos")]
        [OpenApiOperation(operationId: "CambiarEstadoYLiquidarPuntos", tags: new[] { "CambiarEstadoYLiquidarPuntos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Cambia el estado del ususario y liquida puntos")]
        public async Task<IActionResult> CambiarEstadoYLiquidarPuntos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fidelizacion/CambiarEstadoYLiquidarPuntos/{email}")] HttpRequest req, ILogger log, string email)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");
            try
            {
                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }
                var response = await this.puntosApplication.CambiarEstadoYLiquidarPuntos(email);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al cambiar el estado y liquidar punttos:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("UpdateMesYAnio")]
        [OpenApiOperation(operationId: "UpdateMesYAnio", tags: new[] { "UpdateMesYAnio" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los extractos por fechas")]
        public async Task<IActionResult> UpdateMesYAnio(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/UpdateMesYAnio")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<ReporteDto>(requestBody);
                var response = await this.puntosApplication.UpdateMesYAño(data);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los extractos:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetExtractosByUsuarioAndDate")]
        [OpenApiOperation(operationId: "GetExtractosByUsuarioAndDate", tags: new[] { "GetExtractosByUsuarioAndDate" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los extractos por fechas")]
        public async Task<IActionResult> GetExtractosByUsuarioAndDate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/GetExtractosByUsuarioAndDate")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<ReporteDto>(requestBody);
                var response = await this.puntosApplication.GetExtractosByUserAndDate(data);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los extractos:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("UpdateUser")]
        [OpenApiOperation(operationId: "UpdateUser", tags: new[] { "UpdateUser" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los extractos por fechas")]
        public async Task<IActionResult> UpdateUser(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/UpdateUser")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var response = await this.puntosApplication.UpdateUser();
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los extractos:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("UpdateEmpresaYAgencia")]
        [OpenApiOperation(operationId: "UpdateEmpresaYAgencia", tags: new[] { "UpdateEmpresaYAgencia" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los extractos por fechas")]
        public async Task<IActionResult> UpdateEmpresaYAgencia(
            [HttpTrigger(AuthorizationLevel.Anonymous, "Post", Route = "fidelizacion/UpdateEmpresaYAgencia")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var response = await this.puntosApplication.UpdateEmpresaYAgencia();
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los extractos:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("EliminarExtractos")]
        [OpenApiOperation(operationId: "EliminarExtractos", tags: new[] { "EliminarExtractos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los extractos por fechas")]
        public async Task<IActionResult> EliminarExtractos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fidelizacion/EliminarExtractos")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                var response = await this.puntosApplication.GenerateExtratosByFidelizacionPuntos();
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los extractos:" + DateTime.UtcNow.ToString(), ex);
            }
        }


        [FunctionName("RecalcularPuntos")]
        [OpenApiOperation(operationId: "RecalcularPuntos", tags: new[] { "RecalcularPuntos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los extractos por fechas")]
        public async Task<IActionResult> RecalcularPuntos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "fidelizacion/RecalcularPuntos")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                await this.puntosApplication.RecalcularPuntos();
                return new OkResult();
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los extractos:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("CreateRed")]
        [OpenApiOperation(operationId: "CreateRed", tags: new[] { "CreateRed" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Crea la redencion")]
        public async Task<IActionResult> CreateRed(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/CreateRed")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function process ed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<UsuarioRedencion>(requestBody);
                var response = await this.puntosApplication.CreateRedencion(data);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los extractos:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("AddVariable")]
        [OpenApiOperation(operationId: "AddVariable", tags: new[] { "AddVariable" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Guardar la variable")]
        public async Task<IActionResult> AddVariable(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            try
            {
                log.LogInformation($"Puntos:GetPuntos Inicia obtener todos los Puntos. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<VariableDto>(requestBody);

                var variable = await this.puntosApplication.AddVariable(data);
                return new OkObjectResult(variable);

            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetVariableById")]
        [OpenApiOperation(operationId: "GetVariableById", tags: new[] { "GetVariableById" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con las variables")]
        public async Task<IActionResult> GetVariableById(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetVariableById/{id}")] HttpRequest req,
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

                var variable = await this.puntosApplication.GetVariableById(id);

                return new OkObjectResult(variable);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener las variables, Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetVariables")]
        [OpenApiOperation(operationId: "GetVariables", tags: new[] { "GetVariables" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de variables")]
        public async Task<IActionResult> GetVariable(
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

                log.LogInformation($"Puntos:GetVariables Inicia obtener todas las variables. Fecha:{DateTime.UtcNow}");
                var variables = await puntosApplication.GetVariables();
                log.LogInformation($"Puntos:GetVariables finaliza obtener todas las variables sin errores. Fecha:{DateTime.UtcNow}");
                return new OkObjectResult(variables);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los puntos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("DeleteVariableById")]
        [OpenApiOperation(operationId: "DeleteVariableById", tags: new[] { "DeleteVariableById" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Lista de dtos con las variables")]

        public async Task<IActionResult> DeleteVariableById(
           [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteVariableById/{id}")] HttpRequest req,
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


                var variable = await this.puntosApplication.DeleteVariableById(id);

                return new OkObjectResult(variable);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al eliminar los puntos Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("UpdateCorreoInfoPuntos")]
        [OpenApiOperation(operationId: "UpdateCorreoInfoPuntos", tags: new[] { "UpdateCorreoInfoPuntos" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Update correo de infoPuntos")]

        public async Task<IActionResult> UpdateCorreoInfoPuntos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "UpdateCorreoInfoPuntos")] HttpRequest req,
           ILogger log)
        {

            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                if (log is null)
                {
                    throw new ArgumentNullException(nameof(log));
                }


                log.LogInformation($"Puntos:UpdateCorreoInfoPuntos Inicia a hacer update del correo. Fecha:{DateTime.UtcNow}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<UpdateInfoDto>(requestBody);
                var res = await this.puntosApplication.UpdateCorreoInfoPuntos(data);
                return new OkObjectResult(res);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al hacer update del correo:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetMetricasByState")]
        [OpenApiOperation(operationId: "GetMetricasByState", tags: new[] { "GetMetricasByState" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los reportes por fechas")]
        public async Task<IActionResult> GetMetricasByState(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/GetMetricasByState")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<ReporteDto>(requestBody);
                var response = await this.puntosApplication.GetMetricasByState(data);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los reportes:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetMetricasPorDia")]
        [OpenApiOperation(operationId: "GetMetricasPorDia", tags: new[] { "GetMetricasPorDia" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los reportes por fechas")]
        public async Task<IActionResult> GetMetricasPorDia(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/GetMetricasPorDia")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<ReporteDto>(requestBody);
                var response = await this.puntosApplication.GetMetricasPorDia(data);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los reportes:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("GetMetricasGeneral")]
        [OpenApiOperation(operationId: "GetMetricasGeneral", tags: new[] { "GetMetricasGeneral" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los reportes por fechas")]
        public async Task<IActionResult> GetMetricasGeneral(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/GetMetricasGeneral")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<ReporteDto>(requestBody);
                var response = await this.puntosApplication.GetMetricasGeneral(data);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los reportes:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("AddNroGuiaYTransportadoraMasivo")]
        [OpenApiOperation(operationId: "AddNroGuiaYTransportadoraMasivo", tags: new[] { "AddNroGuiaYTransportadoraMasivo" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los reportes por fechas")]
        public async Task<IActionResult> AddNroGuiaYTransportadoraMasivo(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/AddNroGuiaYTransportadoraMasivo")] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<NroPedidoDto[]>(requestBody);
                var response = await this.puntosApplication.AddNroGuiaYTransportadoraMasivo(data);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los reportes:" + DateTime.UtcNow.ToString(), ex);
            }
        }
    }
}

