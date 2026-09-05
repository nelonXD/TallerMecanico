using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using TallerMecanico.Endpoints;
using TallerMecanico.Models;
using TallerMecanico.Services;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudince = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudince,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey))

            };
        });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<TallerMecanicoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("miconexion")));
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddScoped<AuthService>();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapEspecialidadApi();
app.MapServicioApi();
app.MapMecanicoApi();
app.MapModeloApi();
app.MapRepuestoApi();
app.MapVehiculoApi();
app.MapOrdenesTrabajoApi();
app.MapPagoApi();
app.MapClienteApi();
app.MapMarcaApi();
app.MapRolApi();
app.MapUsuarioApi();




app.Run();

