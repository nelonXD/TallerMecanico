using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
namespace TallerMecanico.Endpoints
{
    public static class RolApi
    {
        public static void MapRolApi(this WebApplication app)
        {
            var rol = app.MapGroup("/api/rol").WithTags("Rol");

            rol.MapGet("/",async(TallerMecanicoDbContext db) => await db.Roles.ToListAsync());

            rol.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var rol = await db.Roles.FindAsync(id);
                return rol is not null ? Results.Ok(rol) : Results.NotFound();
            });

            rol.MapPost("/", async (Role rol, TallerMecanicoDbContext db) =>
            {
                db.Roles.Add(rol);
                await db.SaveChangesAsync();
                return Results.Created($"/api/rol/{rol.RolId}", rol);
            });

            rol.MapPut("/{id:int}", async (int id, Role rol, TallerMecanicoDbContext db) =>
            {
                var existingRol = await db.Roles.FindAsync(id);
                if (existingRol is null) return Results.NotFound();
                existingRol.Nombre = rol.Nombre;
                existingRol.Descripcion = rol.Descripcion;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            rol.MapDelete("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var rol = await db.Roles.FindAsync(id);
                if (rol is null) return Results.NotFound();
                db.Roles.Remove(rol);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
