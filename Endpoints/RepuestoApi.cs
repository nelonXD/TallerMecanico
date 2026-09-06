using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;

namespace TallerMecanico.Endpoints
{
    public static class RepuestoApi
    {
        public static void MapRepuestoApi(this WebApplication app)
        {
            var repuesto = app.MapVersionedV1Group("repuestos", "Repuestos");
            
            repuesto.MapGet("/", async(TallerMecanicoDbContext db) => await db.Repuestos.ToListAsync());

            repuesto.MapGet("/{id}", async (int id, TallerMecanicoDbContext db) =>
            {
                var repuesto = await db.Repuestos.FindAsync(id);
                return repuesto is not null ? Results.Ok(repuesto) : Results.NotFound();
            });

            repuesto.MapPost("/", async (RepuestoDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var repuesto = new Repuesto
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Precio = dto.Precio,
                    Stock = dto.Stock
                };

                db.Repuestos.Add(repuesto);
                await db.SaveChangesAsync();
                return Results.Created($"/api/v1/repuestos/{repuesto.RepuestoId}", repuesto);
            });
            
            repuesto.MapPut("/{id}", async (int id, RepuestoDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var repuesto = await db.Repuestos.FindAsync(id);
                if (repuesto is null) return Results.NotFound();
                
                repuesto.Nombre = dto.Nombre;
                repuesto.Descripcion = dto.Descripcion;
                repuesto.Precio = dto.Precio;
                repuesto.Stock = dto.Stock;
                
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
