using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using TallerMecanico.Models;
using TallerMecanico.Repositories;
using Asp.Versioning;
using Asp.Versioning.Builder;

namespace TallerMecanico.Endpoints
{
    public static class ClienteApi
    {
        public static void MapClienteApi(this WebApplication app)
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            var clientes = app.MapGroup("/api/v{version:apiVersion}/clientes")
                .WithApiVersionSet(apiVersionSet)
                .HasApiVersion(new ApiVersion(1, 0))
                .WithTags("Clientes");

            // Requiere autenticación y rol Admin o Mecanico para acceder a clientes
            clientes.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Mecanico" });

            clientes.MapGet("/", async (IClienteRepository repository) =>
            {
                var clientesList = await repository.GetAllAsync();
                return Results.Ok(clientesList);
            });

            clientes.MapGet("/{id:int}", async (int id, IClienteRepository repository) =>
            {
                var cliente = await repository.GetByIdAsync(id);
                return cliente is not null ? Results.Ok(cliente) : Results.NotFound();
            });

            clientes.MapPost("/", async (Cliente cliente, IClienteRepository repository, IValidator<Cliente> validator) =>
            {
                var validationResult = await validator.ValidateAsync(cliente);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                await repository.AddAsync(cliente);
                await repository.SaveChangesAsync();
                return Results.Created($"/api/v1/clientes/{cliente.ClienteId}", cliente);
            });

            clientes.MapPut("/{id:int}", async (int id, Cliente updatedCliente, IClienteRepository repository, IValidator<Cliente> validator) =>
            {
                var validationResult = await validator.ValidateAsync(updatedCliente);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var cliente = await repository.GetByIdAsync(id);
                if (cliente is null) return Results.NotFound();

                cliente.Nombre = updatedCliente.Nombre;
                cliente.Apellido = updatedCliente.Apellido;
                cliente.Telefono = updatedCliente.Telefono;
                cliente.Correo = updatedCliente.Correo;
                cliente.Direccion = updatedCliente.Direccion;

                repository.Update(cliente);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });

            // Solo el rol Admin puede eliminar clientes
            clientes.MapDelete("/{id:int}", async (int id, IClienteRepository repository) =>
            {
                var cliente = await repository.GetByIdAsync(id);
                if (cliente is null) return Results.NotFound();

                repository.Remove(cliente);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
        }
    }
}
