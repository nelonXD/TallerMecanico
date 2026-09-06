using TallerMecanico.Models;
using TallerMecanico.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TallerMecanico.ValidacionesDTO;
using Microsoft.AspNetCore.Authorization;

namespace TallerMecanico.Endpoints
{
    public static class UsuarioApi
    {
        public static void MapUsuarioApi(this WebApplication app)
        {
            var user = app.MapVersionedV1Group("usuarios", "Usuarios");

            user.MapPost("/registro", async (RegistroRequest request, TallerMecanicoDbContext db, AuthService auth) => {
                    if (request.Validar() is { } errorDeValidacion) return errorDeValidacion;

                    var rolMecanico = await db.Roles.SingleOrDefaultAsync(rol => rol.Nombre == "Mecanico");
                    if (rolMecanico is null) return Results.Problem("El rol predeterminado no está configurado.", statusCode: StatusCodes.Status500InternalServerError);

                    var usuario = new Usuario
                    {
                        Nombre = request.Nombre,
                        Correo = request.Correo,
                        RolId = rolMecanico.RolId
                    };

                    usuario.PasswordHash = auth.HashPassword(usuario, request.Password);
                    db.Usuarios.Add(usuario);
                    await db.SaveChangesAsync();
                    
                    return Results.Created($"/api/v1/usuarios/{usuario.UsuarioId}", new { usuario.UsuarioId, usuario.Nombre, usuario.Correo });
                }).AllowAnonymous();

            user.MapPost("/login", async (LoginRequest login, TallerMecanicoDbContext db,
                IConfiguration config) =>
            {
                var usuario = db.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefault(u => u.Nombre == login.NombreUsuario);

                if (login.Validar() is { } errorDeValidacion) return errorDeValidacion;

                if (usuario is null)
                    return Results.Unauthorized();

                var hasher = new PasswordHasher<Usuario>();
                var resultado = hasher.VerifyHashedPassword(
                    usuario, usuario.PasswordHash, login.Password);

                if (resultado == PasswordVerificationResult.Failed)
                    return Results.Unauthorized();

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Role, usuario.Rol.Nombre)

                };
                var jwtKey = config["Jwt:Key"];
                var jwtIssuer = config["Jwt:Issuer"];
                var jwtAudience = config["Jwt:Audience"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    jwtKey ?? throw new InvalidOperationException("Jwt:Key is missing.")));
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(5),
                    signingCredentials: credenciales
                );

                return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }).AllowAnonymous();
        }
        public record LoginRequest(string NombreUsuario, string Password);
    }
}
