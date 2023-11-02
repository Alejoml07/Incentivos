using ApiGateway.Aggregators;
using ApiGateway.Handlers;
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

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuerSigningKey = true,
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
//            ValidAudience = builder.Configuration["JwtSettings:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtSettings:SecuredSecretKey").Value)),
//            ClockSkew = new System.TimeSpan(0)
//        };
//    });

// Configure the HTTP request pipeline.

var app = builder.Build();

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

//app.UseAuthentication();
app.UseCors();
app.UseOcelot().Wait();

app.Run();

