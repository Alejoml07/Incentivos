using Microsoft.Extensions.Configuration;
using PuntosLeonisa.Infraestructure.Core.Agent.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.ExternalServiceInterfaces;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.DTO;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Helpers;

namespace PuntosLeonisa.Fd.Infrastructure.ExternalService
{
    public class EmailExternalServices : IEmailExternalService
    {
        private readonly IHttpClientAgent httpClientAgent;
        private readonly IConfiguration _configuration;
        private readonly ISecurityService securityService;

        public EmailExternalServices(IHttpClientAgent httpClientAgent, IConfiguration configuration,ISecurityService securityService)
        {
            this.httpClientAgent = httpClientAgent;
            this._configuration = configuration;
            this.securityService = securityService;
        }

        public async  Task<GenericResponse<bool>> SendMailForResetPasswordByUser(UsuarioDto data,string urlReset)
        {
            try
            {
                var urlComplete = $"{_configuration["urlBaseReset"]}/{urlReset}";
                var email = new EmailDTO()
                {
                    message = this.securityService.GenerarHTML(urlComplete),
                    recipients = new string[] { data.Email, "danielmg12361@gmail.com" },
                    subject = "Restablecer contraseña"
                };
                var response = this.httpClientAgent.SendMail(email);

                return new GenericResponse<bool>() { Result = true };
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}