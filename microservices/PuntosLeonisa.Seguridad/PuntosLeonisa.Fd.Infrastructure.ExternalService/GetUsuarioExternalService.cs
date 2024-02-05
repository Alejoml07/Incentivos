using Microsoft.Extensions.Configuration;
using PuntosLeonisa.Infraestructure.Core.Agent.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.ExternalServiceInterfaces;
using PuntosLeonisa.Products.Domain.Model;
using PuntosLeonisa.Seguridad.Domain.Service.DTO.Usuarios;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PuntosLeonisa.Fd.Infrastructure.ExternalService
{
    public class GetUsuarioExternalService : IGetUsuarioExternalService
    {
        private readonly IHttpClientAgent httpClientAgent;
        private readonly IConfiguration _configuration;

        public GetUsuarioExternalService(IHttpClientAgent httpClientAgent, IConfiguration configuration)
        {
            this.httpClientAgent = httpClientAgent;
            this._configuration = configuration;
        }
        public async Task<GenericResponse<bool>> GetUsuario(LoginDto login)
        {
            var azf = $"{_configuration["AzfBaseGetUser"]}{_configuration["GetUsuario"]}/{login.Email}";
            var response = await httpClientAgent.GetRequest<GenericResponse<bool>>(new Uri(azf));
            return response;
        }
    }
}
