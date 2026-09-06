using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;

namespace TallerMecanico.Endpoints
{
    public static class MecanicoApi
    {
        public static void MapMecanicoApi(this WebApplication app)
        {
            var mecanico = app.MapVersionedV1Group("mecanicos", "Mecanicos");

            mecanico.MapGet("/", async(TallerMecanicoDbContext db) => await db.Mecanicos.ToListAsync());

            mecanico.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var mecanico = await db.Mecanicos.FindAsync(id);
                return mecanico is not null ? Results.Ok(mecanico) : Results.NotFound();
            });

            mecanico.MapPost("/", async (MecanicoDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var mecanicoEntity = new Mecanico
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Telefono = dto.Telefono,
                    EspecialidadId = dto.EspecialidadId
                };

                db.Mecanicos.Add(mecanicoEntity);
                await db.SaveChangesAsync();
                return Results.Created($"/api/v1/mecanicos/{mecanicoEntity.MecanicoId}", mecanicoEntity);
            });

            mecanico.MapPut("/{id:int}", async (int id, MecanicoDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var mecanicoEntity = await db.Mecanicos.FindAsync(id);
                if (mecanicoEntity is null) return Results.NotFound();
                
                mecanicoEntity.Nombre = dto.Nombre;
                mecanicoEntity.Apellido = dto.Apellido;
                mecanicoEntity.Telefono = dto.Telefono;
                mecanicoEntity.EspecialidadId = dto.EspecialidadId;
                
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
