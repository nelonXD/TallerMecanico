# Evidencia de pruebas de integracion

## Comando

```powershell
dotnet test .\TallerMecanico.csproj
```

## Resultado esperado

```text
Total: 3
Errores: 0
Correctas: 3
```

## Cobertura demostrada

- El documento OpenAPI responde `200` y contiene rutas versionadas en `v1`.
- El recurso principal `ordenes-trabajo` publica GET, POST, PUT y DELETE.
- Un usuario anonimo recibe `401 Unauthorized` al consultar un CRUD protegido.

Las pruebas usan `WebApplicationFactory` y reemplazan SQL Server por una base InMemory para aislar el contrato HTTP.
