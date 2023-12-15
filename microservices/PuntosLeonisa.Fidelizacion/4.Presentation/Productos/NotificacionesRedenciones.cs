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
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using System.Net;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Seguridad.Application.Core;

namespace PuntosLeonisa.Fidelizacion.Function
{
    public  class NotificacionesRedenciones
    {
        private readonly IFidelizacionApplication puntosApplication;
        private readonly GenericResponse<PuntosManualDto> responseError;
        private readonly BadRequestObjectResult puntosApplicationErrorResult;

        public NotificacionesRedenciones(IFidelizacionApplication usuarioApplication)
        {
            puntosApplication = usuarioApplication;
            this.responseError = new GenericResponse<PuntosManualDto>();
            this.puntosApplicationErrorResult = new BadRequestObjectResult(this.responseError);
        }

        private IActionResult GetFunctionError(ILogger log, string logMessage, Exception ex)
        {
            log.LogError(ex, logMessage, null);
            this.responseError.Message = ex.Message;
            this.responseError.IsSuccess = false;
            return this.puntosApplicationErrorResult;
        }


        [FunctionName("EnviarNotificacionCodigoRedencion")]
        [OpenApiOperation(operationId: "EnviarNotificacionRedencion", tags: new[] { "EnviarNotificacionCodigoRedencion" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los puntos por Id")]
        public async Task<IActionResult> EnviarNotificacionCodigoRedencion(
       [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/redencion/notificacion/EnviarNotificacionCodigoRedencion")] HttpRequest req,
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
                var data = JsonConvert.DeserializeObject<SmsDto>(requestBody);
                var response = await this.puntosApplication.SaveCodeAndSendSms(data);
                return new OkObjectResult(new { result = response != null });
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los puntos:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        [FunctionName("ValidacionNotificacionCodigoRedencion")]
        [OpenApiOperation(operationId: "ValidacionNotificacionCodigoRedencion", tags: new[] { "ValidacionNotificacionCodigoRedencion" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<>), Description = "Obtiene los puntos por Id")]
        public async Task<IActionResult> ValidacionNotificacionCodigoRedencion(
     [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/redencion/notificacion/ValidacionNotificacionCodigoRedencion")] HttpRequest req,
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
                var data = JsonConvert.DeserializeObject<SmsDto>(requestBody);
                var response = await this.puntosApplication.ValidateCodeRedencion(data);
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los puntos:" + DateTime.UtcNow.ToString(), ex);
            }
        }
    }
}
