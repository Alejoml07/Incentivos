using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PuntosLeonisa.Domain.Core.Repository;
using PuntosLeonisa.Fd.Infrastructure.ExternalService;
using PuntosLeonisa.Infraestructure.Core.Agent.Agentslmpl;
using PuntosLeonisa.Infraestructure.Core.Agent.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.ExternalServiceInterfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Application;
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.Seguridad.Domain.Interfaces;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using PuntosLeonisa.Seguridad.Infrasctructure.Common.Helpers;
using PuntosLeonisa.Seguridad.Infrasctructure.Repositorie;

[assembly: FunctionsStartup(typeof(Productos.Startup))]
namespace Productos
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            var stringConnection = Environment.GetEnvironmentVariable("accountCosmoName");
            var key = Environment.GetEnvironmentVariable("keySecret");
            var bd = Environment.GetEnvironmentVariable("db");

            builder.Services.AddDbContext<SeguridadContext>(x => x.UseCosmos(stringConnection, bd));

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
            builder.Services.AddScoped<ITokenRepository, TokenRepository>();
            //Add Service
            builder.Services.AddScoped<IHttpClientAgent, HttpClientAgents>();
            builder.Services.AddScoped<IEmailExternalService, EmailExternalServices>();
            builder.Services.AddScoped<IGetUsuarioExternalService, GetUsuarioExternalService>();
            builder.Services.AddScoped<ISecurityService>(provider => new SecurityHelper(key));
            builder.Services.AddScoped<ICircuitBreaker, CircuitBreaker>();
            builder.Services.AddScoped<ITransientRetry, TransientRetry>();

            builder.Services.AddScoped<IUsuarioApplication, SeguridadApplication>();
            builder.Services.AddScoped<IProveedorApplication, ProveedorApplication>();

        }
    }
}

