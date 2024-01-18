using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PuntosLeonisa.fidelizacion.Domain.Service.DTO.PuntosManuales;
using PuntosLeonisa.Fidelizacion.Application.Core.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Seguridad.Application.Core;

namespace PuntosLeonisa.Fidelizacion.Function
{
    public class RedencionServiceBus
    {
        private readonly ILogger<RedencionServiceBus> _logger;
        private readonly IUsuarioInfoPuntosApplication usuarioInfoPuntosApplication;
        private readonly IFidelizacionApplication puntosApplication;
        private readonly GenericResponse<PuntosManualDto> responseError;
        private readonly BadRequestObjectResult puntosApplicationErrorResult;


        public RedencionServiceBus(ILogger<RedencionServiceBus> log, IFidelizacionApplication usuarioInfoPuntosApplication)
        {
            _logger = log;
            this.usuarioInfoPuntosApplication = usuarioInfoPuntosApplication;
            this.puntosApplicationErrorResult = new BadRequestObjectResult(this.responseError);
        }


        [FunctionName("CreateRedencion")]
        public async Task<IActionResult> CreateRedencion(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "fidelizacion/redencion/create")] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("SendMessageFunction: trigger function processed a request.");
                string connectionString = Environment.GetEnvironmentVariable("ServiceBusConnectionString");
                string queueName = Environment.GetEnvironmentVariable("QueueName");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                var data = JsonConvert.DeserializeObject<UsuarioRedencion>(requestBody);

                var serviceBusClient = new QueueClient(connectionString, queueName);
                 
                var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data)));
               

                await serviceBusClient.SendAsync(message);

                log.LogInformation($"Mensaje enviado: {message}");

                return new OkObjectResult(new GenericResponse<bool>() { Result = true});
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los puntos:" + DateTime.UtcNow.ToString(), ex);
            }
        }

        private IActionResult GetFunctionError(ILogger log, string logMessage, Exception ex)
        {
            log.LogError(ex, logMessage, null);
            this.responseError.Message = ex.Message;
            this.responseError.IsSuccess = false;
            return this.puntosApplicationErrorResult;
        }


        [FunctionName("ProcessMessageFunction")]
        public void ProcessMessageFunction(
        [ServiceBusTrigger("queueredenciones", Connection = "ServiceBusConnectionString")] string myQueueItem,
        ILogger log)
        {
            try
            {

                log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

                var data = JsonConvert.DeserializeObject<UsuarioRedencion>(myQueueItem);
                //aqui obtienes el usuarioinfopuntos y le restas los puntos
                
                this.usuarioInfoPuntosApplication.RedencionPuntos(data);

                log.LogInformation("Mensaje procesado correctamente");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error procesando la redencion");
            }

        }

    }

}

