using ApiGateway.Aggregators;
using ApiGateway.Handlers;
using ApiGateway.Middlerware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddOcelot()
    .AddDelegatingHandler<RemoveEncodingDelegatingHandler>(true)
    .AddDelegatingHandler<HeadersHandler>()
    .AddSingletonDefinedAggregator<UsersPostsAggregator>();

builder.Services.AddCors(c =>
{
    c.AddDefaultPolicy(options =>
    {
        options.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes("C3Fg6@2pLm8!pQrS0tVwX2zY&fUjWnZ1")))) // Clave secreta

        //ClockSkew = new System.TimeSpan(0)
    };
});

// Configure the HTTP request pipeline.

var app = builder.Build();

app.Use(async (context, next) =>
{


    bool ValidarUrlOrigen(string urlOrigen)
    {
        // Obtener la URL permitida desde una variable de entorno
        var urlPermitida = Environment.GetEnvironmentVariable("URL_PERMITIDA") ?? "https://www.ejemplo.com"; // Usa un valor predeterminado si no se encuentra la variable

        return urlOrigen.StartsWith(urlPermitida, StringComparison.OrdinalIgnoreCase);
    }
    // Obtener la URL de origen de la petición
    var urlOrigen = context.Request.Headers["Origin"].ToString();


    if (context.Request.Headers.Any(p => p.Key == "keyApi"))
    {
        if (!ValidarUrlOrigen(urlOrigen))
        {
            context.Response.StatusCode = 403; // Prohibido
            await context.Response.WriteAsync("Acceso denegado desde esta URL");
            return;
        }


        if (context.Request.Headers.TryGetValue("keyApi", out var keyApi))
        {
            if (keyApi.ToString() != "bf82a6de6d6c484626ef4a5349aa194f")
            {
                context.Response.StatusCode = 401; // No autorizado
                await context.Response.WriteAsync("Key inválida");
                return;
            }
            else
            {
                context.Request.Headers.Add("Authorization", "Bearer " + "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhbGVqYW5kcm9tdW5vemxlemNhbm9AZ21haWwuY29tIiwiYXVkIjoiUHVibGljIiwiaXNzIjoiYXJiZW1zLmNvbSIsImV4cCI6MTcxMjA2MjcxOX0.0a8oE0JE0WmQigSSDZZXdtBJLxEvqMdkaFge7P5dLjw");

                await next.Invoke();
                // return;
            }
        }
    }
    else if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
    {
        // await context.Response.WriteAsync("Key 4");

        var token = authHeader.ToString().Substring("Bearer ".Length).Trim();
        if (token == "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhbGVqYW5kcm9tdW5vemxlemNhbm9AZ21haWwuY29tIiwiYXVkIjoiUHVibGljIiwiaXNzIjoiYXJiZW1zLmNvbSIsImV4cCI6MTcxMjA2MjcxOX0.0a8oE0JE0WmQigSSDZZXdtBJLxEvqMdkaFge7P5dLjw")
        {
            await next.Invoke();
            return;
        }

        if (!JwtValidator.ValidateToken(token, "C3Fg6@2pLm8!pQrS0tVwX2zY&fUjWnZ1", ref context))
        {
            context.Response.StatusCode = 401; // No autorizado
            await context.Response.WriteAsync("Token inválido o expirado");
            return;
        }
        else
        {
            // await context.Response.WriteAsync("Key 3dasd");

        }

    }
    else
    {
        // await context.Response.WriteAsync("Key 233");

    }

    await next.Invoke();
});

//Obtengo la variable de ambiente.
var env = builder.Configuration.GetSection("env").Value;

//Asigno el archivo Ocelot.json dependiendo del ambiente en el que est� trabajando
if (env.Equals("dev"))
{
    builder.Configuration.AddJsonFile("Ocelot.json", optional: false, reloadOnChange: true);
}
if (env.Equals("prod"))
{
    builder.Configuration.AddJsonFile("Ocelot-Prod.json", optional: false, reloadOnChange: true);
}
if (env.Equals("relese"))
{
    builder.Configuration.AddJsonFile("Ocelot-Relese.json", optional: false, reloadOnChange: true);
}
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseCors();


app.UseOcelot().Wait();

app.Run();