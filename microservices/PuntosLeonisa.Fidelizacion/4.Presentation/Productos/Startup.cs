using System;
using Logistic.Infrastructure.Agents.AgentsImpl;
using Logistic.Infrastructure.Agents.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PuntosLeonisa.Fd.Infrastructure.ExternalService.Services;
using PuntosLeonisa.Fidelizacion.Application;
using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.UnitOfWork;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie;
using PuntosLeonisa.Infraestructure.Core.Agent.AgentsImpl;
using PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Application.Core;

[assembly: FunctionsStartup(typeof(Productos.Startup))]
namespace Productos
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //Add ServiceProxy
            builder.Services.AddScoped<IHttpClientAgent, HttpClientAgents>();
            builder.Services.AddScoped<IUsuarioExternalService, UsuarioExternalServices>();
            builder.Services.AddScoped<ICircuitBreaker, CircuitBreaker>();
            builder.Services.AddScoped<ITransientRetry, TransientRetry>();

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            var stringConnection = Environment.GetEnvironmentVariable("accountCosmoName");
            var bd = Environment.GetEnvironmentVariable("db");


            builder.Services.AddDbContext<FidelizacionContext>(x => x.UseCosmos(stringConnection, bd));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IPuntosManualRepository, PuntosManualRepository>();
            builder.Services.AddScoped<IWishListRepository, WishListRepository>();
            builder.Services.AddScoped<ICarritoRepository, CarritoRepository>();
            builder.Services.AddScoped<IUsuarioInfoPuntosRepository, UsuarioInfoPuntosRepository>();
            builder.Services.AddScoped<IFidelizacionApplication, FidelizacionApplication>();

        }
    }
}

