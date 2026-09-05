using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Endpoints
{
    public static class PagoApi
    {
        public static void MapPagoApi(this WebApplication app)
        {
            var pago = app.MapGroup("/api/pago").WithTags("Pago");
            pago.MapGet("/", async (TallerMecanicoDbContext db) => await db.Pagos.ToListAsync());
            pago.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var pago = await db.Pagos.FindAsync(id);
                return pago is not null ? Results.Ok(pago) : Results.NotFound();
            });
            pago.MapPost("/", async (Pago pago, TallerMecanicoDbContext db) =>
            {
                db.Pagos.Add(pago);
                await db.SaveChangesAsync();
                return Results.Created($"/api/pago/{pago.PagoId}", pago);
            });
            pago.MapPut("/{id:int}", async (int id, Pago updatedPago, TallerMecanicoDbContext db) =>
            {
                var pago = await db.Pagos.FindAsync(id);
                if (pago is null) return Results.NotFound();
                pago.OrdenId = updatedPago.OrdenId;
                pago.MontoTotal = updatedPago.MontoTotal;
                pago.MetodoPago = updatedPago.MetodoPago;
                pago.FechaPago = updatedPago.FechaPago;
                pago.Estado = updatedPago.Estado;
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
