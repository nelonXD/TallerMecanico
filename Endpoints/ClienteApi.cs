using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;

namespace TallerMecanico.Endpoints
{
    public static class ClienteApi
    {
        public static void MapClienteApi(this WebApplication app)
        {
            var clientes = app.MapGroup("/api/clientes").WithTags("Clientes");

            clientes.MapGet("/", async (TallerMecanicoDbContext db) =>
            {
                var clientesList = await db.Clientes.ToListAsync();
                return Results.Ok(clientesList);
            });

            clientes.MapGet("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var cliente = await db.Clientes.FindAsync(id);
                return cliente is not null ? Results.Ok(cliente) : Results.NotFound();
            });

            clientes.MapPost("/", async (Cliente cliente, TallerMecanicoDbContext db) =>
            {
                db.Clientes.Add(cliente);
                await db.SaveChangesAsync();
                return Results.Created($"/api/clientes/{cliente.ClienteId}", cliente);
            });

            clientes.MapPut("/{id:int}", async (int id, Cliente updatedCliente, TallerMecanicoDbContext db) =>
            {
                var cliente = await db.Clientes.FindAsync(id);
                if (cliente is null) return Results.NotFound();
                cliente.Nombre = updatedCliente.Nombre;
                cliente.Apellido = updatedCliente.Apellido;
                cliente.Telefono = updatedCliente.Telefono;
                cliente.Correo = updatedCliente.Correo;
                cliente.Direccion = updatedCliente.Direccion;
                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            clientes.MapDelete("/{id:int}", async (int id, TallerMecanicoDbContext db) =>
            {
                var cliente = await db.Clientes.FindAsync(id);
                if (cliente is null) return Results.NotFound();
                db.Clientes.Remove(cliente);
                await db.SaveChangesAsync();
                return Results.NoContent();
            });
        }
    }
}
