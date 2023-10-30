using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Productos.Startup))]
namespace Productos
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            var stringConnection = Environment.GetEnvironmentVariable("ConnectionStrings");
            builder.Services.AddDbContext<PlatformContext>(x => x.UseSqlServer(stringConnection));

            //Add ServiceProxy
            builder.Services.AddScoped<IHttpClientAgent, HttpClientAgents>();
            //builder.Services.AddScoped<ICircuitBreaker, CircuitBreaker>();
            //builder.Services.AddScoped<ITransientRetry, TransientRetry>();
        }
    }
}

