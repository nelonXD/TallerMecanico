using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
namespace TallerMecanico.Endpoints
{
    public static class EspecialidadApi
    {
        public static void MapEspecialidadApi(this WebApplication app)
        {
            var especialidades = app.MapGroup("/api/especialidad").WithTags("Especialidad");

            especialidades.MapGet("/", async (TallerMecanicoDbContext db) => await db.Especialidades.ToListAsync());



            especialidades.MapPost("/", async (Especialidade especialidad, TallerMecanicoDbContext db) =>
            {
                db.Especialidades.Add(especialidad);
                await db.SaveChangesAsync();
                return Results.Created($"/api/especialidad/{especialidad.EspecialidadId}", especialidad);
            });

            especialidades.MapPut("/{id:int}", async (int id, Especialidade especialidad, TallerMecanicoDbContext db) =>
            {
                var existingEspecialidad = await db.Especialidades.FindAsync(id);
                if (existingEspecialidad is null) return Results.NotFound();
                existingEspecialidad.Nombre = especialidad.Nombre;
                existingEspecialidad.Descripcion = especialidad.Descripcion;
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
