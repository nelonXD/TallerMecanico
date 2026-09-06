using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;

namespace TallerMecanico.Endpoints
{
    public static class ModeloApi
    {
        public static void MapModeloApi(this WebApplication app)
        {
            var grupo = app.MapVersionedV1Group("modelo", "Modelo");
            grupo.MapGet("/", async (TallerMecanicoDbContext context) =>
            {
                var modelos = await context.Modelos.ToListAsync();
                return Results.Ok(modelos);
            });
            grupo.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext context) =>
            {
                var modelo = await context.Modelos.FindAsync(id);
                return modelo is not null ? Results.Ok(modelo) : Results.NotFound();
            });
            grupo.MapPost("/", async (ModeloDTO dto, TallerMecanicoDbContext context) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var modelo = new Modelo
                {
                    Nombre = dto.Nombre,
                    MarcaId = dto.MarcaId
                };

                context.Modelos.Add(modelo);
                await context.SaveChangesAsync();
                return Results.Created($"/api/v1/modelo/{modelo.ModeloId}", modelo);
            });
            grupo.MapPut("/{id:int}", async (int id, ModeloDTO dto, TallerMecanicoDbContext context) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var modelo = await context.Modelos.FindAsync(id);
                if (modelo is null) return Results.NotFound();
                
                modelo.Nombre = dto.Nombre;
                modelo.MarcaId = dto.MarcaId;
                
                await context.SaveChangesAsync();
                return Results.NoContent();
            });
            grupo.MapDelete("/{id:int}", async (int id, TallerMecanicoDbContext context) =>
            {
                var modelo = await context.Modelos.FindAsync(id);
                if (modelo is null) return Results.NotFound();
                context.Modelos.Remove(modelo);
                await context.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
