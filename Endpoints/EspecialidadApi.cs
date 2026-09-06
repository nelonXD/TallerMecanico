using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;
namespace TallerMecanico.Endpoints
{
    public static class EspecialidadApi
    {
        public static void MapEspecialidadApi(this WebApplication app)
        {
            var especialidades = app.MapVersionedV1Group("especialidad", "Especialidad");

            especialidades.MapGet("/", async (TallerMecanicoDbContext db) => await db.Especialidades.ToListAsync());

            especialidades.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var especialidad = await db.Especialidades.FindAsync(id);
                return especialidad is not null ? Results.Ok(especialidad) : Results.NotFound();
            });

            especialidades.MapPost("/", async (EspecialidadDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var especialidad = new Especialidade
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion
                };

                db.Especialidades.Add(especialidad);
                await db.SaveChangesAsync();
                return Results.Created($"/api/v1/especialidad/{especialidad.EspecialidadId}", especialidad);
            });

            especialidades.MapPut("/{id:int}", async (int id, EspecialidadDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var existingEspecialidad = await db.Especialidades.FindAsync(id);
                if (existingEspecialidad is null) return Results.NotFound();
                existingEspecialidad.Nombre = dto.Nombre;
                existingEspecialidad.Descripcion = dto.Descripcion;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            especialidades.MapDelete("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var existingEspecialidad = await db.Especialidades.FindAsync(id);
                if (existingEspecialidad is null) return Results.NotFound();
                db.Especialidades.Remove(existingEspecialidad);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });




        }
    }
}
