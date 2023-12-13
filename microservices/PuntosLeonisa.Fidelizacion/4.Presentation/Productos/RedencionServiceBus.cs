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
using PuntosLeonisa.Fidelizacion.Application.Core.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Seguridad.Application.Core;

namespace PuntosLeonisa.Fidelizacion.Function
{
    public class RedencionServiceBus
    {
        private readonly ILogger<RedencionServiceBus> _logger;
        private readonly IUsuarioInfoPuntosApplication usuarioInfoPuntosApplication;

        public RedencionServiceBus(ILogger<RedencionServiceBus> log, IFidelizacionApplication usuarioInfoPuntosApplication)
        {
            _logger = log;
            this.usuarioInfoPuntosApplication = usuarioInfoPuntosApplication;
        }


        [FunctionName("SendMessageFunction")]
        public async Task<IActionResult> SendMessageFunction(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
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

                return new OkObjectResult($"Mensaje enviado a la cola: {queueName}");
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("Error al enviar el mensaje");
            }
        }

        [FunctionName("ProcessMessageFunction")]
        public void ProcessMessageFunction(
        [ServiceBusTrigger("queueredenciones", Connection = "ServiceBusConnectionString")] string myQueueItem,
        ILogger log)

        {
            try
            {

                log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
                // Aquí puedes añadir la lógica para procesar el mensaje
                // obtener el mensaje y deserializarlo
                var data = JsonConvert.DeserializeObject<UsuarioRedencion>(myQueueItem);
                // aqui obtienes el usuarioinfopuntos y le restas los puntos
                this.usuarioInfoPuntosApplication.RedencionPuntos(data);

                log.LogInformation("Mensaje procesado correctamente");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error");
            }

        }

    }

}

