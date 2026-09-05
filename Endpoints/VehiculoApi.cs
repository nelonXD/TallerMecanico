using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Endpoints
{
    public static class VehiculoAPI
    {
        public static void MapVehiculoApi(this WebApplication app)
        {
            var vehiculo = app.MapGroup("/api/vehiculos").WithTags("Vehiculos");
            vehiculo.MapGet("/", async (TallerMecanicoDbContext db) => await db.Vehiculos.ToListAsync());

            vehiculo.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var vehiculo = await db.Vehiculos.FindAsync(id);
                return vehiculo is not null ? Results.Ok(vehiculo) : Results.NotFound();
            });

            vehiculo.MapPost("/", async (Vehiculo vehiculo, TallerMecanicoDbContext db) =>
            {
                db.Vehiculos.Add(vehiculo);
                await db.SaveChangesAsync();
                return Results.Created($"/api/vehiculos/{vehiculo.VehiculoId}", vehiculo);
            });

            vehiculo.MapPut("/{id:int}", async (int id, Vehiculo updatedVehiculo, TallerMecanicoDbContext db) =>
            {
                var vehiculo = await db.Vehiculos.FindAsync(id);
                if (vehiculo is null) return Results.NotFound();
                vehiculo.Patente = updatedVehiculo.Patente;
                vehiculo.Anio = updatedVehiculo.Anio;
                vehiculo.Color = updatedVehiculo.Color;
                vehiculo.ClienteId = updatedVehiculo.ClienteId;
                vehiculo.ModeloId = updatedVehiculo.ModeloId;
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
