# Archivist Enterprise

Enterprise knowledge assistant with evidence-grounded answers from authorized workspace content.

## Repository layout

- `src/` — .NET 10 API, worker, and layered backend projects.
- `web/` — Nuxt 4 / Vue 3 frontend.
- `docs/` — architecture, handoff, and delivery decisions for contributors.
- `infra/` — local and cloud infrastructure definitions.
- `deploy/` — operational deployment assets.

## Current status

This is an intentionally thin foundation. Authentication, ingestion, and retrieval are not implemented yet. The initial SQL Server schema migration exists, but has not been applied to a database. The database architecture is documented in [docs/database-architecture.md](docs/database-architecture.md).

## Prerequisites

- .NET SDK 10.0.100 or later feature band
- Node.js 20.19+ (or 22.12+) and npm
- Docker Desktop for local dependencies

## Container-first local startup

```sh
docker compose up --build
```

This runs Microsoft SQL Server 2025, the .NET API, the .NET worker, and Nuxt. It does not require a local .NET SDK. Set a strong `MSSQL_SA_PASSWORD` in `.env` before starting. Visit `http://localhost:3000`; API health is at `http://localhost:5080/health`. Set `ARCHIVIST_API_PORT` in `.env` to use another available host port.

See [docs/agent-handoff.md](docs/agent-handoff.md) before beginning a feature.
