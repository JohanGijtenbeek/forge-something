# ERP Solution

## Solution structuur

```
src/
  Erp.Domain/          -- Entities, enums, value objects (geen dependencies)
  Erp.Infrastructure/  -- EF Core, repositories, configuraties
  Erp.Api/             -- Minimal API endpoints
tests/
  Erp.Domain.Tests/
  Erp.Application.Tests/
docker/
  sqlserver/
    init.sh            -- Wacht op SQL Server, laadt schema
    init.sql           -- Database schema (mdata schema)
    seed_low.sql       -- Testdata low omgeving
docker-compose.yml     -- SQL Server (low/medium/high profiel) + Meilisearch
```

## Opstarten

```powershell
# Development omgeving
docker compose --profile low up -d

# SSMS verbinden: localhost,1433 / sa / StrongPassword123!
```

## NuGet packages herstellen

```powershell
dotnet restore
```
