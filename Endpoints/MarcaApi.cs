using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;

namespace TallerMecanico.Endpoints
{
    public static class MarcaApi
    {
        public static void MapMarcaApi(this WebApplication app)
        {
            var marca = app.MapVersionedV1Group("marca", "Marca");
            
            marca.MapGet("/", async (TallerMecanicoDbContext db) => await db.Marcas.ToListAsync());

            marca.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var marca = await db.Marcas.FindAsync(id);
                return marca is not null ? Results.Ok(marca) : Results.NotFound();
            });

            marca.MapPost("/", async (MarcaDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var marca = new Marca
                {
                    Nombre = dto.Nombre
                };

                db.Marcas.Add(marca);
                await db.SaveChangesAsync();
                return Results.Created($"/api/v1/marca/{marca.MarcaId}", marca);
            });

            marca.MapPut("/{id:int}", async (int id, MarcaDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var marca = await db.Marcas.FindAsync(id);
                if (marca is null) return Results.NotFound();
                marca.Nombre = dto.Nombre;
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
