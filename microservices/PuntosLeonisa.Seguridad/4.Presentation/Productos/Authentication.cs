using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Seguridad/Authenticate")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                if (req is null)
                {
                    throw new ArgumentNullException(nameof(req));
                }


                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var login = JsonConvert.DeserializeObject<LoginDto>(requestBody);

                var usuarioAuth = await this.usuarioApplication.Authentication(login);


                //var tokenHandler = new JwtSecurityTokenHandler();
                //var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("keySecret"));
                //var tokenDescriptor = new SecurityTokenDescriptor
                //{
                //    Subject = new ClaimsIdentity(new[]
                //    {
                //    new Claim(ClaimTypes.Email, usuarioAuth.Result.Correo),
                //    // Agrega más claims si es necesario
                //}),
                //    Expires = DateTime.UtcNow.AddDays(7), // Expiración del token
                //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                //};

                //var token = tokenHandler.CreateToken(tokenDescriptor);
                //usuarioAuth.Result.Tkn = tokenHandler.WriteToken(token);
                usuarioAuth.Result.Tkn = this.GenerateToken("dasd", Environment.GetEnvironmentVariable("keySecret"), null, "dasda", null);
                //= functionHelper.GenerateToken(usuarioAuth.Result.Correo);


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


        private  string GenerateToken(string tenantId, string key, string[] scopes, string? documentId, dynamic user, int lifetime = 3600, string ver = "1.0")
        {
            string docId = documentId ?? "";
            DateTime now = DateTime.Now;

            SigningCredentials credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);

            JwtHeader header = new JwtHeader(credentials);
            JwtPayload payload = new JwtPayload
            {
                { "documentId", docId },
                { "scopes", scopes },
                { "tenantId", tenantId },
                { "user", user },
                { "iat", new DateTimeOffset(now).ToUnixTimeSeconds() },
                { "exp", new DateTimeOffset(now.AddSeconds(lifetime)).ToUnixTimeSeconds() },
                { "ver", ver },
                { "jti", Guid.NewGuid() }
            };

            JwtSecurityToken token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}

