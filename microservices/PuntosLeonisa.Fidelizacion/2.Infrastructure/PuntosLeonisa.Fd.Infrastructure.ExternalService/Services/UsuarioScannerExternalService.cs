using Logistic.Infrastructure.Agents.Interfaces;
using Microsoft.Extensions.Configuration;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Redencion;
using PuntosLeonisa.Fidelizacion.Domain.Service.DTO.Scanner;
using PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fd.Infrastructure.ExternalService.Services
{
    public class UsuarioScannerExternalServices : IUsuarioScannerExternalService
    {
        private readonly IHttpClientAgent httpClientAgent;
        private readonly IConfiguration _configuration;

        public UsuarioScannerExternalServices(IHttpClientAgent httpClientAgent, IConfiguration configuration)
        {
            this.httpClientAgent = httpClientAgent;
            this._configuration = configuration;
        }

        public async Task<UsuarioScannerDto> GetUsuarioScanner(PeticionCedulaDto data)
        {
            var azf = $"{_configuration["AzfUsuarioScanner"]}{_configuration["search-clients"]}";
            var response = await httpClientAgent.PostRequestScanner<UsuarioScannerDto, PeticionCedulaDto>(new Uri(azf), data);
            return response;
        }
    }
}
