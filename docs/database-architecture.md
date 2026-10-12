# Database architecture (pending implementation approval)

This document is the design authority for the first EF Core migration. Do not create tables or migrations until this is accepted.

## Scope and conventions

Archivist v1 is a single-tenant application. Tables must not include speculative
`tenant_id` or `organization_id` columns. Entra's tenant ID remains on an external
identity only to verify the identity provider that issued it; it is not an
application tenancy boundary.

All identifiers are application-generated UUIDs mapped to SQL Server
`uniqueidentifier`. Every mutable business record has `created_at` and `updated_at`
(`datetimeoffset(7)`, UTC); soft-deletable records also have `deleted_at`. Use
`snake_case` table and column names, `datetimeoffset(7)` for timestamps, and
`nvarchar(max)` JSON columns only for provider-specific or policy metadata that does
not need relational constraints. Store large source files and extracted text in Blob
storage; the database stores metadata, locations, hashes, and retrieval data.

## Logical schema

| Table | Purpose and important columns | Constraints / indexes |
| --- | --- | --- |
| `users` | Application person: `id`, `display_name`, `email`, `status`, audit timestamps | unique normalized email when present; index active users |
| `external_identities` | Entra mapping: `id`, `user_id`, `provider`, `issuer`, `tenant_id`, `subject_id` | unique `(provider, issuer, subject_id)`; index `user_id` |
| `roles` | Named application roles: `id`, `code`, `name`, `description`, `is_system` | unique `code` |
| `permissions` | Atomic privileges: `id`, `code`, `description` | unique `code` |
| `role_permissions` | Role-to-permission mapping | primary key `(role_id, permission_id)` |
| `workspaces` | Knowledge boundary: `id`, `code`, `name`, `description`, `lifecycle`, `retrieval_profile`, `retrieval_count` | unique `code`; index `(lifecycle, code)` |
| `workspace_role_grants` | A user's role within one workspace: `workspace_id`, `user_id`, `role_id`, `granted_by_user_id`, `granted_at`, `revoked_at` | one active grant per `(workspace_id, user_id, role_id)`; index active grants by user |
| `documents` | Stable logical record: `id`, `workspace_id`, `title`, `kind`, `owner_user_id`, `current_version_id`, timestamps, `deleted_at` | index `(workspace_id, deleted_at, title)` |
| `document_versions` | Immutable accepted source revision: `id`, `document_id`, `version_number`, `status`, `source_uri`, `blob_key`, `content_hash`, `mime_type`, `source_metadata`, `created_by_user_id`, timestamps | unique `(document_id, version_number)` and `(document_id, content_hash)`; index ready versions |
| `document_chunks` | Ordered, citation-addressable excerpts: `id`, `document_version_id`, `ordinal`, `text`, `page_number`, `section_label`, `char_start`, `char_end`, `content_hash` | unique `(document_version_id, ordinal)`; index by version |
| `chunk_embeddings` | Vector per chunk/profile: `id`, `document_chunk_id`, `model`, `dimensions`, `embedding`, `created_at` | unique `(document_chunk_id, model)`; `embedding` is a SQL Server 2025 `vector(n)` with a fixed `n` per embedding model, and v1 uses exact `VECTOR_DISTANCE` search |
| `ingestion_jobs` | Idempotent work for one version: `id`, `document_version_id`, `kind`, `status`, `idempotency_key`, timestamps, `error_code`, `error_detail` | unique `idempotency_key`; index runnable statuses |
| `ingestion_job_attempts` | Immutable attempt history: `id`, `ingestion_job_id`, `attempt_number`, `started_at`, `finished_at`, `outcome`, `detail` | unique `(ingestion_job_id, attempt_number)` |
| `chat_sessions` | User-owned named conversation: `id`, `user_id`, `title`, timestamps, `deleted_at` | index `(user_id, updated_at desc)` |
| `chat_messages` | Ordered user/assistant/system record: `id`, `chat_session_id`, `sequence_number`, `role`, `content`, `retention_state`, `created_at` | unique `(chat_session_id, sequence_number)`; content is nullable when redacted or excluded |
| `message_citations` | Evidence used by an assistant message: `chat_message_id`, `document_chunk_id`, `ordinal`, `excerpt`, `retrieval_score` | unique `(chat_message_id, ordinal)` |
| `workspace_notes` | One personal note per user/workspace: `id`, `workspace_id`, `user_id`, `content`, timestamps | unique `(workspace_id, user_id)` |
| `note_message_links` | Links a note to supporting conversation records | primary key `(workspace_note_id, chat_message_id)` |
| `content_jobs` | Administrator-defined scheduled source task: `id`, `workspace_id`, `name`, `schedule`, `status`, `source_type`, `source_config`, `created_by_user_id` | unique `(workspace_id, name)`; index enabled schedules |
| `content_job_sources` | Explicit approved source locations for a job: `id`, `content_job_id`, `source_uri`, `source_metadata`, `enabled` | unique `(content_job_id, source_uri)` |
| `content_job_runs` | Execution record: `id`, `content_job_id`, `started_at`, `finished_at`, `status`, `trigger`, `detail` | index `(content_job_id, started_at desc)` |
| `audit_events` | Append-only accountability: `id`, `occurred_at`, `actor_user_id`, `action`, `outcome`, `correlation_id`, `entity_type`, `entity_id`, `metadata` | index `(occurred_at desc)`, `(actor_user_id, occurred_at desc)`, and `(entity_type, entity_id)`; no updates/deletes |

## Controlled values

Use SQL Server check constraints initially rather than database enums, so values can
evolve through normal migrations. The initial permitted values are:

- `users.status`: `active`, `disabled`.
- `workspaces.lifecycle`: `draft`, `published`, `archived`.
- `document_versions.status`: `pending`, `processing`, `ready`, `failed`, `superseded`, `deleted`.
- Job and run status: `queued`, `running`, `succeeded`, `failed`, `cancelled`.
- `chat_messages.role`: `user`, `assistant`, `system`.
- `chat_messages.retention_state`: `stored`, `redacted`, `excluded`.

## Referential and lifecycle rules

- A document belongs to exactly one workspace. A version belongs to one document and
  becomes immutable after its content hash is accepted.
- `documents.current_version_id` is optional while the first version is processing;
  it can reference only a `ready` version of the same document, enforced in
  application logic and tests.
- The database must reject hard deletion of documents, versions, chunks, users, and
  evidence referenced by audit or citation records. Use lifecycle status or
  `deleted_at` instead.
- A workspace may be archived but its history remains readable to administrators;
  archived workspaces never participate in global-chat retrieval.
- Chat prompts are stored only when the approved retention policy permits it. A
  redaction replaces content with null and preserves the event's minimal metadata.

## Identity and authorization

```text
users ──< external_identities
users ──< workspace_role_grants >── roles ──< role_permissions >── permissions
users ──< workspace_role_grants >── workspaces
```

- `external_identities` maps the Entra tenant ID and object ID to an application `user`.
- Roles and permissions are application data managed in SQL Server. Entra establishes identity; it does not replace the application authorization model.
- `workspace_role_grants` gives a user a role in one workspace. Start with direct user grants; group grants can be added without changing the global-chat boundary.

## Knowledge content and retrieval

```text
workspaces ──< documents ──< document_versions ──< document_chunks ──< chunk_embeddings
                                       └──< ingestion_jobs ──< ingestion_job_attempts
```

`workspaces` holds name, code, description, lifecycle (`draft`, `published`, `archived`), retrieval profile, and retrieval count. A document version owns source location/page data. Only a `ready` version in a `published` workspace can enter retrieval.

## User data and evidence

```text
users ──< chat_sessions ──< chat_messages ──< message_citations >── document_chunks
users ──< workspace_notes
workspace_notes ──< note_message_links >── chat_messages
```

The initial rule remains one note per user/workspace. Named chat sessions are implemented rather than retaining the original unused table.

## Scheduled content jobs

```text
content_jobs ──< content_job_sources
content_jobs ──< content_job_runs ──> ingestion_jobs
```

Jobs define a schedule and approved source configuration. Each run fetches, validates, and creates a document version and ingestion job; it must not make arbitrary web retrieval available to chat.

## Audit

`audit_events` is append-only and records actor, action, outcome, timestamp, correlation ID, scoped entity references, and policy-safe metadata. Chat prompts require an explicit retention/redaction policy before storing their full text.

## Required access predicate

For every global-chat retrieval query, the API must enforce:

```text
user has workspace role containing chat.query
AND workspace lifecycle is published
AND document version is ready and not deleted
```

## SQL Server retrieval strategy

The v1 deployment target is SQL Server 2025. `chunk_embeddings.embedding` uses its
native `vector(dimensions)` type. Retrieval queries first apply the authorization and
lifecycle predicate, then use `VECTOR_DISTANCE` for exact nearest-neighbor ranking.
The initial EF Core model may map the vector as a provider-specific value and issue
the retrieval statement through a repository-owned raw SQL query; this avoids making
the domain model depend on database vector types.

Do not create a SQL Server vector index in v1. Vector indexing and `VECTOR_SEARCH`
are preview capabilities in SQL Server 2025. Revisit approximate indexing after a
corpus-size benchmark or when the production target is Azure SQL Database, where
those capabilities are generally available.
