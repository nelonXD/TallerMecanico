using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;

namespace TallerMecanico.Endpoints
{
    public static class VehiculoAPI
    {
        public static void MapVehiculoApi(this WebApplication app)
        {
            var vehiculo = app.MapVersionedV1Group("vehiculos", "Vehiculos");
            vehiculo.MapGet("/", async (TallerMecanicoDbContext db) => await db.Vehiculos.ToListAsync());

            vehiculo.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var vehiculo = await db.Vehiculos.FindAsync(id);
                return vehiculo is not null ? Results.Ok(vehiculo) : Results.NotFound();
            });

            vehiculo.MapPost("/", async (VehiculoDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var vehiculo = new Vehiculo
                {
                    Patente = dto.Patente,
                    Anio = dto.Anio,
                    Color = dto.Color,
                    ClienteId = dto.ClienteId,
                    ModeloId = dto.ModeloId
                };

                db.Vehiculos.Add(vehiculo);
                await db.SaveChangesAsync();
                return Results.Created($"/api/v1/vehiculos/{vehiculo.VehiculoId}", vehiculo);
            });

            vehiculo.MapPut("/{id:int}", async (int id, VehiculoDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var vehiculo = await db.Vehiculos.FindAsync(id);
                if (vehiculo is null) return Results.NotFound();
                
                vehiculo.Patente = dto.Patente;
                vehiculo.Anio = dto.Anio;
                vehiculo.Color = dto.Color;
                vehiculo.ClienteId = dto.ClienteId;
                vehiculo.ModeloId = dto.ModeloId;
                
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            vehiculo.MapDelete("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var vehiculo = await db.Vehiculos.FindAsync(id);
                if (vehiculo is null) return Results.NotFound();
                db.Vehiculos.Remove(vehiculo);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });


        }
        

    }
}
