using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
namespace TallerMecanico.Endpoints
{
    public static class ServicioApi
    {
        public static void MapServicioApi(this WebApplication app)
        {
            var servicio = app.MapGroup("/api/servicios").WithTags("Servicios");

            servicio.MapGet("/", async (TallerMecanicoDbContext db) => await db.Servicios.ToListAsync());

            servicio.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var servicio = await db.Servicios.FindAsync(id);
                return servicio is not null ? Results.Ok(servicio) : Results.NotFound();
            });

            servicio.MapPost("/", async (Servicio servicio, TallerMecanicoDbContext db) =>
            {
                db.Servicios.Add(servicio);
                await db.SaveChangesAsync();
                return Results.Created($"/api/servicios/{servicio.ServicioId}", servicio);
            });

            servicio.MapPut("/{id:int}", async (int id, Servicio updatedServicio, TallerMecanicoDbContext db) =>
            {
                var servicio = await db.Servicios.FindAsync(id);
                if (servicio is null) return Results.NotFound();
                servicio.Nombre = updatedServicio.Nombre;
                servicio.Descripcion = updatedServicio.Descripcion;
                servicio.Costo = updatedServicio.Costo;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });


            servicio.MapDelete("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var servicio = await db.Servicios.FindAsync(id);
                if (servicio is null) return Results.NotFound();
                db.Servicios.Remove(servicio);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
