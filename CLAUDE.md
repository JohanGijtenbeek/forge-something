# ForgeSomething ERP — Claude Code Context

## Language

English is the primary language for all code, documentation, commits, and communication.

## Project overview

Custom ERP system for a metalworking company. Replaces a WinForms application from 2016.
Currently in POC phase — Party domain serves as the foundation for orders and articles.

## Stack

### Backend

- **.NET 9** — Minimal API
- **Dapper** — no EF Core, deliberate choice for control and performance
- **SQL Server** — running in Docker for development
- **MediatR** — commands and events (CQRS-light)
- **Meilisearch** — global search
- **Scalar** — OpenAPI UI at `/scalar`

### Frontend

- **React + Vite + TypeScript**
- **Tailwind CSS**
- **TanStack Query** — server state, no Redux
- **React Router**
- **Axios**

### Tooling

- **k6** — load and stress testing
- **Bogus** — seed data generation
- **Docker Compose** — three profiles: low (1433), medium (1434), high (1435)

## Solution structure

```
erp-solution/
  src/
    Erp.Domain/          — Entities, commands, events, interfaces
    Erp.Infrastructure/  — Dapper repositories, handlers, search, snapshots
    Erp.Api/             — Minimal API endpoints, Program.cs
    Erp.EventConsumer/   — MassTransit consumers (audit, search, SignalR)
    Erp.Seeder/          — Bogus console seeder
    Erp.Web/             — React + Vite frontend
  tests/
  docker/
    sqlserver/
      init.sql                — Schema (mdata + audit schemas)
      init.sh                 — Startup script
      seed_organizations.sql  — Low profile seed
      seed_persons.sql        — Low profile seed
  docker-compose.yml
  k6/
    tests/
      smoke.js
      load.js
      stress.js
  .vscode/
    tasks.json
```

## Architecture principles

### Event sourcing light

Commands → Handlers → Domain Events → MediatR publish → multiple handlers

```
CreateOrganizationCommand
  → CreateOrganizationHandler
    → PartyRepository.SaveAsync()
    → publishes PartyCreatedEvent
      → AuditPartyCreatedHandler    (audit.event_log)
      → MaterializePartyHistory     (audit.party_history)
      → IndexPartyOnCreatedHandler  (Meilisearch)
```

### Projections

- **Hot set** (`mdata.parties`) — current state, fast
- **History** (`audit.party_history`) — materialized history
- **Snapshots** (`audit.party_snapshots`) — for efficient event replay
- **Event log** (`audit.event_log`) — append-only, source of truth

### Immutability

Bank accounts, addresses and contact methods are never modified — only added or deactivated. This is a deliberate domain decision.

### SCD approach

No SCD Type 2 in projection tables. Events are the historical truth. Orders contain a snapshot of relevant data at the time of creation.

## Database schema

### Schemas

- `mdata` — master data (parties, roles, addresses etc.)
- `audit` — event log, history, snapshots

### Party domain tables

```sql
mdata.parties
mdata.person_details
mdata.organization_details
mdata.customer_roles          -- sequence: seq_debtor_number
mdata.supplier_roles          -- sequence: seq_supplier_number
mdata.party_addresses         -- address_type_id: 1=Postal, 2=Delivery, 3=Invoice
mdata.party_contact_methods   -- contact_method_type_id: 1=Phone, 2=Email, 3=Mobile
mdata.party_bank_accounts
mdata.party_relationships
```

### Audit tables

```sql
audit.event_log
audit.party_history
audit.party_snapshots
```

## Conventions

### Backend

- **Dapper snake_case mapping** — `DefaultTypeMap.MatchNamesWithUnderscores = true` in `DbConnectionFactory`
- **File-scoped namespaces** — everywhere
- **Records for DTOs** — request/response records in `PartyDtos.cs`
- **TypedResults** — always use in endpoints for typed OpenAPI
- **Transactions** in repository for multi-table writes
- **IRequest/INotification** — commands return a value, events are void

### New domains follow this pattern:

1. Domain entities in `Erp.Domain/{Domain}/`
2. Commands in `Commands/` subdirectory
3. Events in `Events/` subdirectory
4. Repository interface in domain
5. Dapper repository implementation in Infrastructure
6. Command handlers in Infrastructure
7. Event handlers (audit, search, history) in Infrastructure
8. Endpoints in Api with TypedResults
9. DTOs and mapper in Api
10. React Query hooks in frontend
11. API functions in `src/api/`
12. Pages in `src/pages/{domain}/`

### Frontend

- **React Query keys** as constants in hooks file
- **Invalidation** after mutations via `queryClient.invalidateQueries`
- **Axios client** in `src/api/client.ts`
- **Types** in `src/types/api.ts` — 1-to-1 with backend DTOs

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
DELETE /api/parties/{id}           -- soft delete
POST   /api/parties/{fromId}/relationships
```

### Articles

```
GET    /api/articles?page&pageSize&search&categoryId&articleType&includeInactive
GET    /api/articles/{id}
GET    /api/articles/{id}/history
POST   /api/articles
PUT    /api/articles/{id}
DELETE /api/articles/{id}          -- soft delete

GET    /api/articles/{id}/bom
POST   /api/articles/{id}/bom
PUT    /api/articles/{id}/bom/{lineId}
DELETE /api/articles/{id}/bom/{lineId}

GET    /api/article-categories
POST   /api/article-categories

GET    /api/units-of-measure
POST   /api/units-of-measure
```

### Search

```
GET  /api/search?q={query}&limit={limit}
POST /api/search/reindex
```

### Meta

```
GET /health
GET /scalar
```

## Rate limiting

- **Concurrency limiter** — max 50 concurrent, queue 25
- **Sliding window** — max 300 req/min per IP, 6 segments, queue 10
- **429 response** — JSON with `retryAfterSeconds`

## Seeder

Console application in `Erp.Seeder`.

```powershell
dotnet run --project src/Erp.Seeder -- low     # 50 orgs + 50 persons
dotnet run --project src/Erp.Seeder -- medium  # 500 orgs + 100 persons
dotnet run --project src/Erp.Seeder -- high    # 5000 orgs + 500 persons
```

- Clears database before seeding
- Bulk indexes into Meilisearch
- Reproducible via seed value 42
- Name generator uses shuffled pool system

## VS Code tasks

Available via Terminal → Run Task:

- `Dev: start infrastructure` — restart Docker (down -v + up)
- `Dev: start services` — API, frontend and browser in parallel
- `Seeder: low/medium/high`
- `k6: smoke/load/stress`
- `Docker: stop`
- `API: build`

## Git conventions

### Conventional Commits

```
<type>(<scope>): <description>
```

Examples:

```
feat(parties): add update organization endpoint
fix(seeder): resolve duplicate company names
chore(docker): add redis to compose profiles
docs(claude): update pending items
refactor(repository): extract address mapping
test(k6): add medium profile stress test
perf(api): add response compression
```

Types: `feat`, `fix`, `chore`, `docs`, `refactor`, `test`, `perf`, `style`

Scopes: `parties`, `orders`, `articles`, `auth`, `seeder`, `docker`, `frontend`, `api`, `search`, `db`

### GitFlow

- `main` — production, always stable
- `develop` — integration branch, base for new features
- `feature/` — new functionality, branched from develop
- `fix/` — bug fixes, branched from develop
- `docs/` — documentation and convention updates, branched from develop
- `release/` — release preparation, branched from develop

Examples:

```
feature/articles-domain
feature/order-creation
feature/auth-keycloak
fix/meilisearch-city-null
docs/readme-and-conventions
release/0.1.0
```

## Instructions for Claude Code

- Always use context7 for up-to-date library documentation
- Always follow existing patterns — review similar files before implementing anything new
- New backend domains follow the Party domain pattern — see `Erp.Domain/Parties/` and `Erp.Infrastructure/Handlers/`
- New frontend pages follow the pattern of `PartiesPage.tsx` and `PartyDetailPage.tsx`
- New endpoints always use `TypedResults` and follow `PartyEndpoints.cs`
- Never write boilerplate that already exists — reuse `DbConnectionFactory`, `apiClient`, existing hooks
- Update the todo list after completing a feature
- Add new endpoints to the API section after implementation
- Add known issues if you encounter them
- Never modify architecture principles or git conventions without explicit instruction
- Always work on a dedicated branch — never commit to an existing unrelated branch; create a new branch from `develop` before starting any change
- Always commit CLAUDE.md changes separately with `docs(claude): ...`
- Update README.md whenever startup steps, stack, solution structure, or API endpoints change — commit together with the feature or fix that caused the change

## Pending

- [x] Articles domain
- [ ] Orders domain
- [ ] Auth (IdentityServer or Keycloak)
- [ ] Add/edit addresses and contact methods via UI
- [ ] Pagination on list endpoints
- [ ] Redis hot set
- [ ] Cold/archive set business rules
- [ ] Medium/high k6 tests

## Known issues

- `Party.cs` uses `null!` in private constructor for Dapper reconstruction
- `changed_by` is a placeholder `"system"` everywhere — requires auth implementation
- Snapshot triggers: event-count (50), state-based (deactivate), scheduled (24h)
- `DefaultTypeMap.MatchNamesWithUnderscores = true` is essential for Dapper snake_case mapping
- Output caching deliberately omitted due to real-time multi-user requirements
