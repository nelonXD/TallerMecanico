using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Endpoints
{
    public static class MarcaApi
    {
        public static void MapMarcaApi(this WebApplication app)
        {
            var marca = app.MapGroup("/api/marca").WithTags("Marca");
            
            marca.MapGet("/", async (TallerMecanicoDbContext db) => await db.Marcas.ToListAsync());

            marca.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var marca = await db.Marcas.FindAsync(id);
                return marca is not null ? Results.Ok(marca) : Results.NotFound();
            });

            marca.MapPost("/", async (Marca marca, TallerMecanicoDbContext db) =>
            {
                db.Marcas.Add(marca);
                await db.SaveChangesAsync();
                return Results.Created($"/api/marca/{marca.MarcaId}", marca);
            });

            marca.MapPut("/{id:int}", async (int id, Marca updatedMarca, TallerMecanicoDbContext db) =>
            {
                var marca = await db.Marcas.FindAsync(id);
                if (marca is null) return Results.NotFound();
                marca.Nombre = updatedMarca.Nombre;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            marca.MapDelete("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var marca = await db.Marcas.FindAsync(id);
                if (marca is null) return Results.NotFound();
                db.Marcas.Remove(marca);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
