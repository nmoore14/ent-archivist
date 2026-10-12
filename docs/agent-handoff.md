# Contributor handoff

## Read first

1. [architecture.md](architecture.md)
2. [database-architecture.md](database-architecture.md)
3. [decisions.md](decisions.md)

## Current implementation state

- The repository has a container-first development stack. Run `docker compose up --build`; no local .NET SDK is required. The local environment used to create this skeleton did not have the .NET SDK installed, so no native .NET build was executed.
- `web/package-lock.json` defines the pinned frontend dependency graph. Keep it synchronized with `package.json`; use `npm ci` in containers and CI.
- Microsoft SQL Server 2025 local Compose configuration and the initial EF Core schema migration exist; the migration has not been applied.
- The API currently exposes only `/health` and `/api/v1/status`.
- The Nuxt page is a temporary global-chat shell, not a functional chat.

## Safe work boundaries

- Start database work only after the data model is approved. Implement it first in `Domain`, then `Application`, then `Infrastructure` migrations.
- Do not implement workspace selection in the user chat UI.
- Do not put secrets in `appsettings*.json`, committed environment files, or frontend runtime config.
- Preserve append-only audit semantics and authorization predicates in API/application logic.

## Suggested next work order

1. Confirm schema/retention choices; add domain models, EF mappings, and initial migration.
2. Entra authentication and database-backed authorization policies.
3. Workspace/document CRUD plus auditable upload and ingestion states.
4. Queue-backed indexing and SQL Server vector retrieval.
5. Grounded chat, citations, sessions, notes, search, and scheduled content jobs.
