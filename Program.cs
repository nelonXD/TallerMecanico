using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using System.Threading.RateLimiting;
using FluentValidation;
using Asp.Versioning;
using TallerMecanico.Endpoints;
using TallerMecanico.Models;
using TallerMecanico.Services;
using TallerMecanico.Repositories;
using TallerMecanico.Middlewares;
using TallerMecanico.Validators;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Seguridad: Autenticacion JWT ---
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

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
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("Jwt:Key is missing.")))
            };
        });
builder.Services.AddAuthorization();

// --- 2. Acceso a Datos: DbContext y Repositorios ---
builder.Services.AddDbContext<TallerMecanicoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("miconexion")));

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IOrdenesTrabajoRepository, OrdenesTrabajoRepository>();
builder.Services.AddScoped<AuthService>();

// --- 3. Validacion (FluentValidation) ---
builder.Services.AddValidatorsFromAssemblyContaining<ClienteValidator>();

// --- 4. OWASP Mitigations: Rate Limiting & Versioning & CORS ---
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Ejemplo de origen permitido
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// --- Documentacion OpenAPI ---
builder.Services.AddOpenApi();

var app = builder.Build();

// --- OWASP Mitigations: Middlewares ---
app.UseExceptionHandling(); // Manejo global de excepciones para no exponer stack traces
app.UseRequestLogging();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts(); // HSTS para HTTPS estricto
}

app.UseCors("AllowFrontend");
app.UseRateLimiter(); // Mitigacion DoS
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// --- Endpoints ---
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

// Requerido para pruebas de integracion con WebApplicationFactory
public partial class Program { }
