using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.DotNet.PlatformAbstractions;

[assembly: FunctionsStartup(typeof(Productos.Startup))]
namespace Productos
{

    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
     
        }
    }
}

