using Logistic.Infrastructure.Agents.AgentsImpl;
using Logistic.Infrastructure.Agents.Interfaces;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PuntosLeonisa.Fd.Infrastructure.ExternalService.Services;
using PuntosLeonisa.Fidelizacion.Domain.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.Interfaces;
using PuntosLeonisa.Fidelizacion.Domain.Service.UnitOfWork;
using PuntosLeonisa.Fidelizacion.Infrasctructure.Repositorie;
using PuntosLeonisa.Infraestructure.Core.Agent.AgentsImpl;
using PuntosLeonisa.Infrasctructure.Core.ExternaServiceInterfaces;
using PuntosLeonisa.Infrasctructure.Core.Repository;
using PuntosLeonisa.infrastructure.Persistence.CosmoDb;
using PuntosLeonisa.Seguridad.Application;
using PuntosLeonisa.Seguridad.Application.Core;
using PuntosLeonisa.Seguridad.Domain.Service.Interfaces;
using PuntosLeonisa.Seguridad.Infrasctructure.Repositorie;
using System;

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

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IMovimientoPuntosRepository, MovimientoPuntosRepository>();
            builder.Services.AddScoped<IWishListRepository, WishListRepository>();
            builder.Services.AddScoped<IPuntoDeVentaRepository, PuntoDeVentaRepository>();
            builder.Services.AddScoped<ICarritoRepository, CarritoRepository>();
            builder.Services.AddScoped<IUsuarioInfoPuntosRepository, UsuarioInfoPuntosRepository>();
            builder.Services.AddScoped<IFidelizacionApplication, PuntosLeonisa.Fidelizacion.Application.FidelizacionApplication>();
            builder.Services.AddScoped<IExtractosRepository, ExtractosRepository>();
            builder.Services.AddScoped<IVariableRepository, VariableRepository>();
            builder.Services.AddScoped<IPuntoVentaVarRepository, PuntoVentaVarRepository>();
            builder.Services.AddScoped<IAsignacionRepository, AsignacionRepository>();


            //Add ServiceProxy
            builder.Services.AddScoped<IHttpClientAgent, HttpClientAgents>();
            builder.Services.AddScoped<ICircuitBreaker, CircuitBreaker>();
            builder.Services.AddScoped<ITransientRetry, TransientRetry>();
            builder.Services.AddScoped<IUsuarioExternalService, UsuarioExternalServices>();
            builder.Services.AddScoped<IProductoExternalService, ProductoExternalServices>();
            builder.Services.AddScoped<IOrdenOPExternalService, OrdenOPExternalServices>();
            builder.Services.AddScoped<IPuntoDeVentaApplication, PuntoDeVentaApplication>();

        }
    }
}

