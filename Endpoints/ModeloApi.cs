using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Endpoints
{
    public static class ModeloApi
    {
        public static void MapModeloApi(this WebApplication app)
        {
            var grupo = app.MapGroup("/api/modelo").WithTags("Modelo");
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
            grupo.MapPost("/", async (Modelo modelo, TallerMecanicoDbContext context) =>
            {
                context.Modelos.Add(modelo);
                await context.SaveChangesAsync();
                return Results.Created($"/api/modelo/{modelo.MarcaId}", modelo);
            });
            grupo.MapPut("/{id:int}", async (int id, Modelo updatedModelo, TallerMecanicoDbContext context) =>
            {
                var modelo = await context.Modelos.FindAsync(id);
                if (modelo is null) return Results.NotFound();
                modelo.Nombre = updatedModelo.Nombre;
                modelo.MarcaId = updatedModelo.MarcaId;
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
