using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;
namespace TallerMecanico.Endpoints
{
    public static class ServicioApi
    {
        public static void MapServicioApi(this WebApplication app)
        {
            var servicio = app.MapVersionedV1Group("servicios", "Servicios");

            servicio.MapGet("/", async (TallerMecanicoDbContext db) => await db.Servicios.ToListAsync());

            servicio.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var servicio = await db.Servicios.FindAsync(id);
                return servicio is not null ? Results.Ok(servicio) : Results.NotFound();
            });

            servicio.MapPost("/", async (ServicioDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var servicio = new Servicio
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Costo = dto.Costo
                };

                db.Servicios.Add(servicio);
                await db.SaveChangesAsync();
                return Results.Created($"/api/v1/servicios/{servicio.ServicioId}", servicio);
            });

            servicio.MapPut("/{id:int}", async (int id, ServicioDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var servicio = await db.Servicios.FindAsync(id);
                if (servicio is null) return Results.NotFound();
                
                servicio.Nombre = dto.Nombre;
                servicio.Descripcion = dto.Descripcion;
                servicio.Costo = dto.Costo;
                
                await db.SaveChangesAsync();
                return Results.NoContent();
            });


            servicio.MapDelete("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var servicio = await db.Servicios.FindAsync(id);
                if (servicio is null) return Results.NotFound();
                db.Servicios.Remove(servicio);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
