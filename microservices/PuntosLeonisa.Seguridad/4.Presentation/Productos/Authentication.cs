using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Jose;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Productos.FunctionHelper;
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Seguridad.Function
{
    public class Authentication
    {
        private readonly ILogger<Authentication> _logger;
        private readonly IUsuarioApplication usuarioApplication;
        private readonly GenericResponse<UsuarioDto> responseError;
        private readonly BadRequestObjectResult productoApplicationErrorResult;
        private readonly FunctionHelper functionHelper;

        public Authentication(ILogger<Authentication> log, IUsuarioApplication usuarioApplication)
        {
            _logger = log;
            this.usuarioApplication = usuarioApplication;
            this.responseError = new GenericResponse<UsuarioDto>();
            this.productoApplicationErrorResult = new BadRequestObjectResult(this.responseError);
            this.functionHelper = new FunctionHelper(Environment.GetEnvironmentVariable("keySecret"));
        }

        private IActionResult GetFunctionError(ILogger log, string logMessage, Exception ex)
        {
            log.LogError(ex, logMessage, null);
            this.responseError.Message = ex.Message;
            this.responseError.IsSuccess = false;
            return this.productoApplicationErrorResult;
        }

        public Exception GetArgumentNullException()
        {
            return new ArgumentNullException(nameof(_logger));
        }

        [FunctionName("Authenticate")]
        [OpenApiOperation(operationId: "Authentication", tags: new[] { "Authenticate" })]
        [OpenApiParameter(name: "Authenticate", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Authenticate** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Authenticate(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Seguridad/Authenticate")] HttpRequest req, ILogger log)
        {
            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var login = JsonConvert.DeserializeObject<LoginDto>(requestBody);
                log.LogInformation($"Authentication:Authentication Inicia a loguear al ususario. Fecha:{DateTime.UtcNow}");
                GenericResponse<UsuarioResponseLiteDto> usuarioAuth = await usuarioApplication.Authentication(login);
                log.LogInformation($"Authentication:Authentication Termina y loguea al usuario. Fecha:{DateTime.UtcNow}");
                usuarioAuth.Result.Tkn = CreateToken(login.Email, "Public", "arbems.com" );

                return new OkObjectResult(usuarioAuth);
            }
            catch (UnauthorizedAccessException uex)
            {
                return new UnauthorizedObjectResult(new GenericResponse<UsuarioResponseLiteDto>()
                {
                    IsSuccess = false,
                    Message = uex.Message,
                    Result = null
                });
            }
            catch (Exception ex)
            {
                return GetFunctionError(_logger, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }


        public static string CreateToken(string email, string audience, string issuer)
        {
            var payload = new Dictionary<string, object>
                {
                    { "sub", email },
                    { "aud", audience },
                    { "iss", issuer },
                    { "exp", DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds() }
                };

            var secretKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("keySecret") ?? "defaultSecret")); // Clave secreta

            string token = JWT.Encode(payload, Encoding.UTF8.GetBytes(secretKey), JwsAlgorithm.HS256);
            return token;
        }

        [FunctionName("ValidarCorreo")]
        [OpenApiOperation(operationId: "ValidarCorreo", tags: new[] { "Seguridad/ValidarCorreo" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(GenericResponse<IEnumerable<LoginDto>>), Description = "Verifica si el correo existe")]
        public async Task<IActionResult> GetUsuarioos(
           [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Seguridad/ValidarCorreo")] HttpRequest req,
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

                log.LogInformation($"Seguridad: ValidarCorreo valida si el correo existe. Fecha:{DateTime.UtcNow}");

                var exist = JsonConvert.DeserializeObject<LoginDto>(await new StreamReader(req.Body).ReadToEndAsync());
                var response = await usuarioApplication.ValidarCorreo(exist);              
                log.LogInformation($"Seguridad: ValidarCorreo termina de validar si el correo existe. Fecha:{DateTime.UtcNow}");
                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return GetFunctionError(log, "Error al obtener los usuarios Fecha:" + DateTime.UtcNow.ToString(), ex);
            }
        }

    }
}

