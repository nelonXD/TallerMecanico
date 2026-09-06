# Informe tecnico

## Arquitectura y acceso a datos

El proyecto usa ASP.NET Core Minimal APIs. Los endpoints se separan por recurso, los DTO concentran las reglas de validacion y los servicios contienen la autenticacion. La inyeccion de dependencias registra el DbContext, los repositorios y AuthService con alcance Scoped.

Entity Framework Core con SQL Server mapea las entidades del dominio. El patron Repository abstrae el acceso a datos para clientes y ordenes de trabajo; el resto de los recursos utiliza el DbContext inyectado.

La migracion `20260906003020_InitialCreate` fue generada y aplicada a `TallerMecanicoDB`. Como las tablas ya existian, se registro la migracion en `__EFMigrationsHistory` como linea base, sin ejecutar operaciones destructivas.

## API y versionamiento

La API publica OpenAPI automaticamente en desarrollo y usa Scalar como interfaz de documentacion. Todos los recursos estan disponibles mediante rutas versionadas en v1, por ejemplo `/api/v1/clientes`, `/api/v1/ordenes-trabajo` y `/api/v1/especialidad`.

## Seguridad: mitigaciones OWASP API

1. Broken Authentication: JWT firmado, validacion de emisor, audiencia, firma y expiracion. El login solo genera un token si la verificacion del hash de contrasena es exitosa.
2. Broken Object Level Authorization: los CRUD administrativos exigen rol `Admin`; clientes y ordenes de trabajo exigen `Admin` o `Mecanico`, y sus eliminaciones requieren `Admin`.
3. Unrestricted Resource Consumption: limitador global por IP, con maximo de 100 solicitudes por minuto.
4. Security Misconfiguration: CORS restringido al origen configurado, redireccion HTTPS y HSTS fuera de desarrollo.
5. Improper Inventory Management / exposicion de secretos: las configuraciones locales se ignoran en Git y existe `appsettings.example.json` sin valores confidenciales.
6. Improper Error Handling: middleware global registra la excepcion y evita devolver detalles internos fuera del entorno de desarrollo.
7. Input validation: DTO con Data Annotations y MiniValidation; las entradas invalidas responden con HTTP 400. El registro asigna el rol `Mecanico` en el servidor y no acepta `RolId` desde el cliente.

## Roles y flujo de autenticacion

`POST /api/v1/usuarios/registro` y `POST /api/v1/usuarios/login` son publicos. El registro crea usuarios con el rol predeterminado `Mecanico`; la asignacion de `Admin` debe realizarse por administracion de base de datos o un proceso controlado. El login devuelve un JWT con el claim de rol.

Los recursos maestros requieren `Admin`. Clientes y ordenes de trabajo requieren `Admin` o `Mecanico`; eliminar clientes u ordenes requiere `Admin`.

## Evidencia verificable

Comandos ejecutados:

```text
dotnet build .\TallerMecanico.csproj --no-restore
dotnet test .\TallerMecanico.csproj --no-restore
dotnet ef database update --project .\TallerMecanico.csproj --no-build
```

Resultado actual: compilacion correcta y 3 pruebas de integracion correctas. Las pruebas verifican el contrato OpenAPI versionado, el CRUD del recurso principal y que un CRUD administrativo rechaza solicitudes anonimas.

Configurar los secretos locales a partir de `appsettings.example.json`, mediante User Secrets o variables de entorno. Nunca se deben versionar claves JWT ni contrasenas de SQL Server.

## Pendientes de evidencia academica

La validacion del caso de negocio por la docente y la exposicion final son actividades externas al repositorio. Deben completarse y adjuntarse como acta, firma, presentacion o captura antes de la entrega.
