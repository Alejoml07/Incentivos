using Logistic.Infrastructure.Agents.Interfaces;
using Microsoft.Extensions.Configuration;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces;
using PuntosLeonisa.Products.Domain.Model;

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
    }
}
