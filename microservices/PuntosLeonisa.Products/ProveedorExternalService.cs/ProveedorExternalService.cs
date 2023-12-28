using Microsoft.Extensions.Configuration;
using PuntosLeonisa.Infraestructure.Core.Agent.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.ExternalServiceInterfaces;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Products.Infrasctructure.Common.Communication;

namespace PuntosLeonisa.Fd.Infrastructure.ExternalService
{
    public class ProveedorExternalServices : IProveedorExternalService
    {
        private readonly IHttpClientAgent httpClientAgent;
        private readonly IConfiguration _configuration;

        public ProveedorExternalServices(IHttpClientAgent httpClientAgent, IConfiguration configuration)
        {
            this.httpClientAgent = httpClientAgent;
            this._configuration = configuration;
        }

        public async Task<GenericResponse<ProveedorLiteDto>> GetProveedorByNit(string nit)
        {
            var azf = $"{_configuration["AzfBaseUser"]}{_configuration["GetProveedorByNit"]}/{nit}";
            var response = await httpClientAgent.GetRequest<GenericResponse<ProveedorLiteDto>>(new Uri(azf));
            return response;
        }
    }
}