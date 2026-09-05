using TallerMecanico.Models;
using TallerMecanico.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TallerMecanico.Endpoints
{
    public static class UsuarioApi
    {
        public static void MapUsuarioApi(this WebApplication app)
        {
            var user = app.MapGroup("/api/Usuario").WithTags("Usuarios");

            user.MapPost("/registro", async (Usuario usuario, string password,
                TallerMecanicoDbContext db, AuthService auth) => {
                    usuario.PasswordHash = auth.HashPassword(usuario, password);
                    db.Usuarios.Add(usuario);
                    await db.SaveChangesAsync();
                    return Results.Created("/api/usuarios", usuario);
                });

            user.MapPost("/login", async (LoginRequest login, TallerMecanicoDbContext db,
                IConfiguration config) =>
            {
                var usuario = db.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefault(u => u.Nombre == login.NombreUsuario);

                if (usuario is null)
                    return Results.Unauthorized();

                var hasher = new PasswordHasher<Usuario>();
                var resultado = hasher.VerifyHashedPassword(
                    usuario, usuario.PasswordHash, login.Password);

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, usuario.Nombre),
                    new Claim(ClaimTypes.Role, usuario.Rol.Nombre)

                };
                var jwtKey = config["Jwt:Key"];
                var jwtIssuer = config["Jwt:Issuer"];
                var jwtAudience = config["Jwt:Audience"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: jwtIssuer,
                    audience: jwtAudience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(5),
                    signingCredentials: credenciales
                );

                return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            });
        }
        record LoginRequest(string NombreUsuario, string Password);
    }
}
