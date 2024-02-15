using Logistic.Infrastructure.Agents.Interfaces;
using Microsoft.Extensions.Configuration;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces;

namespace PuntosLeonisa.Fd.Infrastructure.ExternalService.Services
{
    public class ProductoExternalServices : IProductoExternalService
    {
        private readonly IHttpClientAgent httpClientAgent;
        private readonly IConfiguration _configuration;

        public ProductoExternalServices(IHttpClientAgent httpClientAgent, IConfiguration configuration)
        {
            this.httpClientAgent = httpClientAgent;
            this._configuration = configuration;
        }

        public Task<GenericResponse<ProductoRefence>> GetProductByEAN(string ean)
        {
            var azf = $"{_configuration["AzfBaseProduct"]}{_configuration["GetProductByEAN"]}/{ean}";
            var response = httpClientAgent.GetRequest<GenericResponse<ProductoRefence>>(new Uri(azf));
            return response;
        }

        public async Task<GenericResponse<IEnumerable<bool>>> UpdateInventory(ProductoRefence[] data)
        {
            var azf = $"{_configuration["AzfBaseProduct"]}{_configuration["UpdateInventory"]}";
            var response = await httpClientAgent.PostRequest<GenericResponse<IEnumerable<bool>>, ProductoRefence[]>(new Uri(azf),data);
            return response;
        }

        
    }
}
