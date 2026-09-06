# Guion para la exposicion final

## 1. Caso de negocio

El taller necesita centralizar clientes, vehiculos, mecanicos, ordenes de trabajo, servicios, repuestos y pagos. La orden de trabajo es el recurso principal porque conecta el vehiculo, cliente y mecanico responsable.

## 2. Arquitectura

- ASP.NET Core Minimal APIs.
- Entity Framework Core con SQL Server.
- DbContext, entidades, relaciones y migraciones.
- Repositories para clientes y ordenes de trabajo.
- Middleware propio para logging y manejo de excepciones.

## 3. API

Mostrar `/openapi/v1.json` y una ruta como `/api/v1/ordenes-trabajo`. Explicar los verbos GET, POST, PUT y DELETE, los DTO y los codigos HTTP.

## 4. Seguridad

Mostrar el login JWT, los roles `Admin` y `Mecanico`, la proteccion de endpoints, la validacion de entradas, rate limiting, CORS, HTTPS/HSTS y el middleware de errores.

## 5. Base de datos

Mostrar el modelo relacional, `TallerMecanicoDbContext`, la migracion `InitialCreate` y `__EFMigrationsHistory`.

## 6. Pruebas

Ejecutar `dotnet test .\TallerMecanico.csproj` y mostrar el resultado de las 3 pruebas correctas.

## Evidencia pendiente

Adjuntar la validacion del caso de negocio firmada o confirmada por la docente y la presentacion final del equipo. Este archivo es solo el material de apoyo tecnico.
