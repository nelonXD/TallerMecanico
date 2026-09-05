using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Endpoints
{
    public static class RepuestoApi
    {
        public static void MapRepuestoApi(this WebApplication app)
        {
            var repuesto = app.MapGroup("/api/repuestos").WithTags("Repuestos");
            
            repuesto.MapGet("/", async(TallerMecanicoDbContext db) => await db.Repuestos.ToListAsync());

            repuesto.MapGet("/{id}", async (int id, TallerMecanicoDbContext db) =>
            {
                var repuesto = await db.Repuestos.FindAsync(id);
                return repuesto is not null ? Results.Ok(repuesto) : Results.NotFound();
            });

            repuesto.MapPost("/", async (Repuesto repuesto, TallerMecanicoDbContext db) =>
            {
                db.Repuestos.Add(repuesto);
                await db.SaveChangesAsync();
                return Results.Created($"/api/repuestos/{repuesto.RepuestoId}", repuesto);
            });
            
            repuesto.MapPut("/{id}", async (int id, Repuesto updatedRepuesto, TallerMecanicoDbContext db) =>
            {
                var repuesto = await db.Repuestos.FindAsync(id);
                if (repuesto is null) return Results.NotFound();
                repuesto.Nombre = updatedRepuesto.Nombre;
                repuesto.Descripcion = updatedRepuesto.Descripcion;
                repuesto.Precio = updatedRepuesto.Precio;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            repuesto.MapDelete("/{id}", async (int id, TallerMecanicoDbContext db) =>
            {
                var repuesto = await db.Repuestos.FindAsync(id);
                if (repuesto is null) return Results.NotFound();
                db.Repuestos.Remove(repuesto);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
