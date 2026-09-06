using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;
namespace TallerMecanico.Endpoints
{
    public static class RolApi
    {
        public static void MapRolApi(this WebApplication app)
        {
            var rol = app.MapVersionedV1Group("rol", "Rol");

            rol.MapGet("/",async(TallerMecanicoDbContext db) => await db.Roles.ToListAsync());

            rol.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var rol = await db.Roles.FindAsync(id);
                return rol is not null ? Results.Ok(rol) : Results.NotFound();
            });

            rol.MapPost("/", async (RolDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var rol = new Role
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion
                };

                db.Roles.Add(rol);
                await db.SaveChangesAsync();
                return Results.Created($"/api/v1/rol/{rol.RolId}", rol);
            });

            rol.MapPut("/{id:int}", async (int id, RolDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var existingRol = await db.Roles.FindAsync(id);
                if (existingRol is null) return Results.NotFound();
                
                existingRol.Nombre = dto.Nombre;
                existingRol.Descripcion = dto.Descripcion;
                
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
