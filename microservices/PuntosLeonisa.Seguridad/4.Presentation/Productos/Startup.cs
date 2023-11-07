using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PuntosLeonisa.Domain.Core.Repository;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Application;
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.Seguridad.Domain.Interfaces;
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
            var bd = Environment.GetEnvironmentVariable("db");

            builder.Services.AddDbContext<SeguridadContext>(x => x.UseCosmos(stringConnection, bd));

            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();

            builder.Services.AddScoped<IUsuarioApplication, SeguridadApplication>();
            builder.Services.AddScoped<IProveedorApplication, ProveedorApplication>();


            //Add ServiceProxy
            //builder.Services.AddScoped<IHttpClientAgent, HttpClientAgents>();
            //builder.Services.AddScoped<ICircuitBreaker, CircuitBreaker>();
            //builder.Services.AddScoped<ITransientRetry, TransientRetry>();
        }
    }
}

