using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Endpoints
{
    public static class MecanicoApi
    {
        public static void MapMecanicoApi(this WebApplication app)
        {
            var mecanico = app.MapGroup("/api/mecanicos").WithTags("Mecanicos");

            mecanico.MapGet("/", async(TallerMecanicoDbContext db) => await db.Mecanicos.ToListAsync());

            mecanico.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var mecanico = await db.Mecanicos.FindAsync(id);
                return mecanico is not null ? Results.Ok(mecanico) : Results.NotFound();
            });

            mecanico.MapPost("/", async (Mecanico mecanico, TallerMecanicoDbContext db) =>
            {
                db.Mecanicos.Add(mecanico);
                await db.SaveChangesAsync();
                return Results.Created($"/api/mecanicos/{mecanico.MecanicoId}", mecanico);
            });

            mecanico.MapPut("/{id:int}", async (int id, Mecanico updatedMecanico, TallerMecanicoDbContext db) =>
            {
                var mecanico = await db.Mecanicos.FindAsync(id);
                if (mecanico is null) return Results.NotFound();
                mecanico.Nombre = updatedMecanico.Nombre;
                mecanico.Especialidad = updatedMecanico.Especialidad;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            mecanico.MapDelete("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var mecanico = await db.Mecanicos.FindAsync(id);
                if (mecanico is null) return Results.NotFound();
                db.Mecanicos.Remove(mecanico);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

        }
    }
}
