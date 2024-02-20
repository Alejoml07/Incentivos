using Logistic.Infrastructure.Agents.Interfaces;
using Microsoft.Extensions.Configuration;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.DTO;
using PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces;
using PuntosLeonisa.Products.Domain.Model;
using System.Net.Http;
using System.Web;
using System.Xml;

namespace PuntosLeonisa.Fd.Infrastructure.ExternalService.Services
{
    public class UsuarioExternalServices : IUsuarioExternalService
    {
        private readonly IHttpClientAgent httpClientAgent;
        private readonly IConfiguration _configuration;

        public UsuarioExternalServices(IHttpClientAgent httpClientAgent, IConfiguration configuration)
        {
            this.httpClientAgent = httpClientAgent;
            this._configuration = configuration;
        }
        public async Task<GenericResponse<Usuario>> GetUserLiteByCedula(string cedula)
        {
            var azf = $"{_configuration["AzfBaseUser"]}{_configuration["GetUserLiteByCedula"]}/{cedula}";
            var response = await httpClientAgent.GetRequest<GenericResponse<Usuario>>(new Uri(azf));
            return response;
        }

        public async Task<GenericResponse<Usuario>> GetUserByEmail(string email)
        {
            var azf = $"{_configuration["AzfBaseUser"]}{_configuration["GetUserLiteByEmail"]}/{email}";
            var response = await httpClientAgent.GetRequest<GenericResponse<Usuario>>(new Uri(azf));
            return response;
        }

        public async Task<bool> SendSmsWithCode(SmsDto data)
        {
            var token = data.Codigo;
            var mensajeToCode = $"Mis suenos a un clic te dice que tu codigo para la redencion es: {token}";
            return await this.SendSms(data.Usuario, mensajeToCode);

        }

        private async Task<bool> SendSms(Usuario usuario, string mensajeToCode)
        {
            try
            {
                var urlSms = $"{_configuration["UrlSms"]}";
                var message = HttpUtility.UrlEncode(mensajeToCode);

                urlSms = urlSms.Replace("{phone}", usuario.Celular);
                urlSms = urlSms.Replace("{message}", message);
                var response = await httpClientAgent.GetRequestXml<Response>(new Uri(urlSms));
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(response);

                XmlNode statusMessageNode = xmlDoc?.SelectSingleNode("//statusmessage[text()='Message acepted for delivery']");

                if (statusMessageNode != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Manejo de excepciones
                Console.WriteLine($"Error al enviar SMS: {ex.Message}");
                return false;
            }
        }

        public async Task<GenericResponse<bool>> SendSmsWithMessage(Usuario usuario,string message)
        {
            var response = new GenericResponse<bool>();
            var result = await this.SendSms(usuario, message);
            response.Result = result;
            return response;
        }

        public Task<GenericResponse<bool>> UserSendEmailWithMessage(UsuarioRedencion data)
        {
            try
            {
                if (data.Usuario.TipoUsuario == "Asesoras Vendedoras")
                {
                    var email = new EmailDTO()
                    {
                        Message = data.GenerarHTML(),
                        Recipients = new string[] { data?.Usuario?.Email, "danielmg12361@gmail.com" },
                        Subject = "Redención de premio"
                    };
                    var response = this.httpClientAgent.SendMail(email);

                    return Task.FromResult(new GenericResponse<bool>() { Result = true });
                }
                else
                {
                    var email = new EmailDTO()
                    {
                        Message = data.GenerarHTML(),
                        Recipients = new string[] { data?.Usuario?.Email, "danielmg12361@gmail.com"},
                        Subject = "Redención de premio"
                    };
                    var response = this.httpClientAgent.SendMail(email);

                    return Task.FromResult(new GenericResponse<bool>() { Result = true });
                }
                
                
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Task<GenericResponse<bool>> UserSendEmailWithMessageAndState(UsuarioRedencion data)
        {
            try
            {
                
                var email = new EmailDTO()
                {
                    Message = data.GenerarHTMLCambioEstado(data.ProductosCarrito.FirstOrDefault().ProveedorLite.Id),
                    Recipients = new string[] { data?.Usuario?.Email, "danielmg12361@gmail.com" },
                    Subject = "Cambios estado premio"
                };
                var response = this.httpClientAgent.SendMail(email);

                return Task.FromResult(new GenericResponse<bool>() { Result = true });
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Task<GenericResponse<bool>> SendMailGeneric(EmailDTO emailData)
        {
            try
            {
               
                var response = this.httpClientAgent.SendMail(emailData);

                return Task.FromResult(new GenericResponse<bool>() { Result = true });
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<GenericResponse<bool>> CambiarEstado(string email)
        {
            var azf = $"{_configuration["AzfBaseUser"]}{_configuration["CambiarEstado"]}/{email}";
            var response = httpClientAgent.GetRequest<GenericResponse<bool>>(new Uri(azf));
            return response;
        }

        
        public Task<GenericResponse<IEnumerable<Usuario>>> GetUsuarios()
        {
            var azf = $"{_configuration["AzfBaseUser"]}{_configuration["GetUsuarios"]}";
            var response = httpClientAgent.GetRequest<GenericResponse<IEnumerable<Usuario>>>(new Uri(azf));
            return response;
        }

     
    }
}
