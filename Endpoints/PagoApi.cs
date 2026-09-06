using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.ValidacionesDTO;

namespace TallerMecanico.Endpoints
{
    public static class PagoApi
    {
        public static void MapPagoApi(this WebApplication app)
        {
            var pago = app.MapVersionedV1Group("pago", "Pago");
            pago.MapGet("/", async (TallerMecanicoDbContext db) => await db.Pagos.ToListAsync());
            pago.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var pago = await db.Pagos.FindAsync(id);
                return pago is not null ? Results.Ok(pago) : Results.NotFound();
            });
            pago.MapPost("/", async (PagoDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var pago = new Pago
                {
                    OrdenId = dto.OrdenId,
                    MontoTotal = dto.MontoTotal,
                    MetodoPago = dto.MetodoPago,
                    FechaPago = dto.FechaPago,
                    Estado = dto.Estado
                };

                db.Pagos.Add(pago);
                await db.SaveChangesAsync();
                return Results.Created($"/api/v1/pago/{pago.PagoId}", pago);
            });
            pago.MapPut("/{id:int}", async (int id, PagoDTO dto, TallerMecanicoDbContext db) =>
            {
                if (dto.Validar() is { } errorDeValidacion) return errorDeValidacion;

                var pago = await db.Pagos.FindAsync(id);
                if (pago is null) return Results.NotFound();
                
                pago.OrdenId = dto.OrdenId;
                pago.MontoTotal = dto.MontoTotal;
                pago.MetodoPago = dto.MetodoPago;
                pago.FechaPago = dto.FechaPago;
                pago.Estado = dto.Estado;
                
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
            pago.MapDelete("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var pago = await db.Pagos.FindAsync(id);
                if (pago is null) return Results.NotFound();
                db.Pagos.Remove(pago);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
