using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PuntosLeonisa.Fidelizacion.Application;
using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie;
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

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            var stringConnection = Environment.GetEnvironmentVariable("accountCosmoName");
            var bd = Environment.GetEnvironmentVariable("db");

            builder.Services.AddDbContext<FidelizacionContext>(x => x.UseCosmos(stringConnection, bd));

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IPuntosManualRepository, PuntosManualRepository>();
            builder.Services.AddScoped<IWishListRepository, WishListRepository>();
            builder.Services.AddScoped<IFidelizacionApplication, FidelizacionApplication>();


            //Add ServiceProxy
            //builder.Services.AddScoped<IHttpClientAgent, HttpClientAgents>();
            //builder.Services.AddScoped<ICircuitBreaker, CircuitBreaker>();
            //builder.Services.AddScoped<ITransientRetry, TransientRetry>();
        }
    }
}

