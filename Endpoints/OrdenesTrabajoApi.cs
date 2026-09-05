using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Endpoints
{
    public static class OrdenesTrabajoApi
    {
        public static void MapOrdenesTrabajoApi(this WebApplication app)
        {
            var ordenesTrabajo = app.MapGroup("/api/ordenes-trabajo");
            ordenesTrabajo.MapGet("/", async (TallerMecanicoDbContext db) =>
            {
                var ordenes = await db.OrdenesTrabajos.ToListAsync();
                return Results.Ok(ordenes);
            });
            ordenesTrabajo.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var orden = await db.OrdenesTrabajos.FindAsync(id);
                return orden is not null ? Results.Ok(orden) : Results.NotFound();
            });
            ordenesTrabajo.MapPost("/", async (OrdenesTrabajo orden, TallerMecanicoDbContext db) =>
            {
                db.OrdenesTrabajos.Add(orden);
                await db.SaveChangesAsync();
                return Results.Created($"/api/ordenes-trabajo/{orden.OrdenId}", orden);
            });
            ordenesTrabajo.MapPut("/{id:int}", async (int id, OrdenesTrabajo updatedOrden, TallerMecanicoDbContext db) =>
            {
                var orden = await db.OrdenesTrabajos.FindAsync(id);
                if (orden is null) return Results.NotFound();
                orden.Observaciones = updatedOrden.Observaciones;
                orden.FechaIngreso = updatedOrden.FechaIngreso;
                orden.Estado = updatedOrden.Estado;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
            ordenesTrabajo.MapDelete("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var orden = await db.OrdenesTrabajos.FindAsync(id);
                if (orden is null) return Results.NotFound();
                db.OrdenesTrabajos.Remove(orden);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
