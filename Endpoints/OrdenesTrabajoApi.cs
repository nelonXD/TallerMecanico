using Microsoft.AspNetCore.Authorization;
using FluentValidation;
using TallerMecanico.Models;
using TallerMecanico.Repositories;
using Asp.Versioning;
using Asp.Versioning.Builder;

namespace TallerMecanico.Endpoints
{
    public static class OrdenesTrabajoApi
    {
        public static void MapOrdenesTrabajoApi(this WebApplication app)
        {
            var apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();

            var ordenesTrabajo = app.MapGroup("/api/v{version:apiVersion}/ordenes-trabajo")
                .WithApiVersionSet(apiVersionSet)
                .HasApiVersion(new ApiVersion(1, 0))
                .WithTags("OrdenesTrabajo");

            // Requiere autenticación y rol Admin o Mecanico
            ordenesTrabajo.RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Mecanico" });

            ordenesTrabajo.MapGet("/", async (IOrdenesTrabajoRepository repository) =>
            {
                var ordenes = await repository.GetOrdenesConDetallesAsync();
                return Results.Ok(ordenes);
            });

            ordenesTrabajo.MapGet("/{id:int}", async (int id, IOrdenesTrabajoRepository repository) =>
            {
                var orden = await repository.GetOrdenConDetallesByIdAsync(id);
                return orden is not null ? Results.Ok(orden) : Results.NotFound();
            });

            ordenesTrabajo.MapPost("/", async (OrdenesTrabajo orden, IOrdenesTrabajoRepository repository, IValidator<OrdenesTrabajo> validator) =>
            {
                var validationResult = await validator.ValidateAsync(orden);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                await repository.AddAsync(orden);
                await repository.SaveChangesAsync();
                return Results.Created($"/api/v1/ordenes-trabajo/{orden.OrdenId}", orden);
            });

            ordenesTrabajo.MapPut("/{id:int}", async (int id, OrdenesTrabajo updatedOrden, IOrdenesTrabajoRepository repository, IValidator<OrdenesTrabajo> validator) =>
            {
                var validationResult = await validator.ValidateAsync(updatedOrden);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }

                var orden = await repository.GetByIdAsync(id);
                if (orden is null) return Results.NotFound();
                
                orden.Observaciones = updatedOrden.Observaciones;
                orden.FechaIngreso = updatedOrden.FechaIngreso;
                orden.Estado = updatedOrden.Estado;
                orden.ClienteId = updatedOrden.ClienteId;
                orden.VehiculoId = updatedOrden.VehiculoId;
                orden.MecanicoId = updatedOrden.MecanicoId;

                repository.Update(orden);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            });

            // Solo Admin puede eliminar
            ordenesTrabajo.MapDelete("/{id:int}", async (int id, IOrdenesTrabajoRepository repository) =>
            {
                var orden = await repository.GetByIdAsync(id);
                if (orden is null) return Results.NotFound();
                
                repository.Remove(orden);
                await repository.SaveChangesAsync();
                return Results.NoContent();
            }).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
        }
    }
}
