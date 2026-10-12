# Architecture

## Product boundary

Archivist exposes one global chat to standard users. It searches only document chunks in workspaces for which the caller has an effective `chat.query` permission. Users do not select a workspace in the chat UI. Administrators manage workspaces, documents, access grants, and content jobs.

## Runtime shape

```text
Nuxt web app → ASP.NET Core API → Microsoft SQL Server 2025
                    ├──────────→ Blob storage
                    ├──────────→ Azure AI Foundry
                    └──────────→ queue ← .NET worker
```

The API owns identity mapping, authorization, query filtering, audit emission, and HTTP/OpenAPI contracts. The worker owns asynchronous ingestion, reindexing, and scheduled content-job execution. The UI never decides what content a caller may retrieve.

## Layering

| Project | Responsibility | Must not depend on |
|---|---|---|
| `Archivist.Domain` | Core entities, value objects, domain contracts | Infrastructure, HTTP, EF Core |
| `Archivist.Application` | Use cases and authorization-aware interfaces | API, infrastructure implementation |
| `Archivist.Contracts` | Public request/response types | Infrastructure |
| `Archivist.Infrastructure` | EF Core, Microsoft SQL Server, queues, Blob, Foundry adapters | API |
| `Archivist.Api` | HTTP, auth middleware, OpenAPI, SSE | Direct database access |
| `Archivist.Worker` | Background job orchestration | HTTP endpoint logic |

## Invariants

1. Every data query is constrained by user access and workspace state.
2. Every response citation points to an authorized immutable document version and source location.
3. Source text is untrusted prompt input; it never overrides system policy.
4. All state-changing operations and chat requests emit audit events.
5. Ingestion and scheduled-content tasks are retriable, idempotent background jobs.
