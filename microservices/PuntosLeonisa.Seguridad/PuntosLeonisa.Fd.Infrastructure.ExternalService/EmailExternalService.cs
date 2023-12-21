using Microsoft.Extensions.Configuration;
using PuntosLeonisa.Infraestructure.Core.Agent.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.ExternalServiceInterfaces;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.DTO;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Helpers;

namespace PuntosLeonisa.Fd.Infrastructure.ExternalService
{
    public class EmailExternalServices : IEmailExternalService
    {
        private readonly IHttpClientAgent httpClientAgent;
        private readonly IConfiguration _configuration;



        public EmailExternalServices(IHttpClientAgent httpClientAgent, IConfiguration configuration)
        {
            this.httpClientAgent = httpClientAgent;
            this._configuration = configuration;
        }

        public Task<GenericResponse<bool>> UserSendEmailWithMessage(UsuarioRedencion data)
        {
            try
            {
                var email = new EmailDTO()
                {
                    Message = SecurityHelper.GenerarHTML(),
                    Recipients = new string[] { data.Usuario.Email, "danielmg12361@gmail.com" },
                    Subject = "Redención de premio"
                };
                var response = this.httpClientAgent.SendMail(email);

                return Task.FromResult(new GenericResponse<bool>() { Result = true });
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}