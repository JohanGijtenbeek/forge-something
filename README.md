# ForgeSomething ERP

Custom ERP system for a metalworking company. Replaces a WinForms application from 2016. Currently in POC phase — Party domain serves as the foundation for orders and articles.

## Stack

| Layer | Technology |
|---|---|
| API | .NET 9 Minimal API, MediatR, Dapper, SQL Server |
| Messaging | RabbitMQ, MassTransit 9 |
| Events | Erp.EventConsumer (Worker + SignalR hub) |
| Search | Meilisearch |
| Frontend | React + Vite + TypeScript, Tailwind CSS, TanStack Query |
| Infra | Docker Compose, k6 |

## Prerequisites

- Docker Desktop
- .NET 9 SDK
- Node.js 20+

## Getting started

### 1. Start infrastructure

Starts SQL Server, Meilisearch, and RabbitMQ. Schema is applied automatically via `init.sql`.

```powershell
docker compose --profile low up -d
```

Profiles: `low` (port 1433), `medium` (1434), `high` (1435).

### 2. Seed data

```powershell
dotnet run --project src/Erp.Seeder -- low      # 50 orgs + 50 persons
dotnet run --project src/Erp.Seeder -- medium   # 500 orgs + 100 persons
dotnet run --project src/Erp.Seeder -- high     # 5000 orgs + 500 persons
```

### 3. Start services

```powershell
# Restore dependencies first (once)
dotnet restore
cd src/Erp.Web && npm install

# Start API
dotnet run --project src/Erp.Api

# Start event consumer (separate terminal)
dotnet run --project src/Erp.EventConsumer

# Start frontend (separate terminal)
cd src/Erp.Web && npm run dev
```

### VS Code tasks (alternative)

Use **Terminal → Run Task** for all of the above:

| Task | Action |
|---|---|
| `Dev: start infrastructure` | Restart Docker (down -v + up) |
| `Seeder: low / medium / high` | Seed the database |
| `Dev: start services` | Start API, consumer, frontend, and open browser |
| `Docker: stop` | Stop all containers |
| `API: build` | Build the API project |
| `k6: smoke / load / stress` | Run k6 load tests |

## URLs

| Service | URL |
|---|---|
| Frontend | http://localhost:5173 |
| API | http://localhost:5272 |
| OpenAPI (Scalar) | http://localhost:5272/scalar |
| Health check | http://localhost:5272/health |
| Event Consumer (SignalR) | http://localhost:5002 |
| Meilisearch | http://localhost:7700 |
| RabbitMQ management | http://localhost:15672 — user: `erp`, password: `erp_password` |
| SQL Server | localhost,1433 — user: `sa`, password: `StrongPassword123!` |

## Solution structure

```
erp-solution/
  src/
    Erp.Domain/          — Entities, commands, events, interfaces
    Erp.Infrastructure/  — Dapper repositories, handlers, search, snapshots
    Erp.Api/             — Minimal API endpoints, Program.cs
    Erp.EventConsumer/   — RabbitMQ consumers, SignalR hub, audit/search side-effects
    Erp.Seeder/          — Bogus console seeder
    Erp.Web/             — React + Vite frontend
  tests/
  docker/
    sqlserver/
      init.sql           — Schema (mdata + audit schemas)
      init.sh            — Startup script
  docker-compose.yml
  k6/
    tests/
      smoke.js
      load.js
      stress.js
  .vscode/
    tasks.json
```

## API endpoints

### Parties

```
GET    /api/parties
GET    /api/parties/customers
GET    /api/parties/suppliers
GET    /api/parties/{id}
GET    /api/parties/{id}/history
POST   /api/parties/organizations
POST   /api/parties/persons
PUT    /api/parties/{id}/organization
PUT    /api/parties/{id}/person
DELETE /api/parties/{id}
```

### Search

```
GET  /api/search?q={query}&limit={limit}
POST /api/search/reindex
```
