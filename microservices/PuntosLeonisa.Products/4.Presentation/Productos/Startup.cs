using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PuntosLeonisa.Fd.Infrastructure.ExternalService;
using PuntosLeonisa.Infraestructure.Core.Agent.Agentslmpl;
using PuntosLeonisa.Infraestructure.Core.Agent.Interfaces;
using PuntosLeonisa.Infrasctructure.Core.ExternalServiceInterfaces;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Products.Application;
using PuntosLeonisa.Products.Application.Core;
using PuntosLeonisa.Products.Domain.Interfaces;
using PuntosLeonisa.Products.Domain.Service.Interfaces;
using PuntosLeonisa.Products.Infrasctructure.Repositorie;

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

            builder.Services.AddDbContext<ProductContext>(x => x.UseCosmos(stringConnection, bd));

            //builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddTransient<IProductoRepository, ProductoRepository>();
            builder.Services.AddTransient<IBannerRepository, BannerRepository>();
            builder.Services.AddTransient<ILogInventarioRepository, LogInventarioRepository>();
            builder.Services.AddScoped<IProductApplication, ProductosApplication>();



            


            //Add ServiceProxy
            builder.Services.AddScoped<IHttpClientAgent, HttpClientAgents>();
            builder.Services.AddScoped<ICircuitBreaker, CircuitBreaker>();
            builder.Services.AddScoped<ITransientRetry, TransientRetry>();
            builder.Services.AddScoped<IProveedorExternalService, ProveedorExternalServices>();
        }
    }
}

