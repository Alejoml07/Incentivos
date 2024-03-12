using Logistic.Infrastructure.Agents.Interfaces;
using Microsoft.Extensions.Configuration;
using PuntosLeonisa.Fidelizacion.Domain.Model;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Common.Communication;
using PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fd.Infrastructure.ExternalService.Services
{
    public class OrdenOPExternalServices : IOrdenOPExternalService
    {
        private readonly IHttpClientAgent httpClientAgent;
        private readonly IConfiguration _configuration;

        public OrdenOPExternalServices(IHttpClientAgent httpClientAgent, IConfiguration configuration)
        {
            this.httpClientAgent = httpClientAgent;
            this._configuration = configuration;
        }
        public async Task<bool> EnviarOrdenOP(OrdenOP ordenOP)
        {
            var azf = $"{_configuration["AzfBaseOrder"]}{_configuration["receiveOrder"]}";
            var response = await httpClientAgent.PostRequest<bool, OrdenOP>(new Uri(azf), ordenOP);
            return response;
        }

        public async Task<int> GetNroOrdenOP(NroPedidoOP nroOrden)
        {
            var azf = $"{_configuration["AzfBaseOrder"]}{_configuration["GenerateConsecutiveOrderRDL"]}";
            var response = await httpClientAgent.PostRequest<int, NroPedidoOP>(new Uri(azf), nroOrden);
            return response;
        }
    }
    
}
