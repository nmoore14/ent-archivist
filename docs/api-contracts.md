# API contract groundwork

These endpoints establish the frontend integration surface. They are intentionally
unavailable (`503 Feature not configured`) until Entra identity, database-backed
authorization, and approved retrieval/persistence are in place. No endpoint returns
demo knowledge or accepts unauthenticated chat prompts.

| Endpoint | Contract | Future behavior |
| --- | --- | --- |
| `GET /api/v1/knowledge/search?query=&kind=` | `KnowledgeSearchPageResponse` | Searches metadata visible to the caller. |
| `POST /api/v1/chat/queries` | `ChatQueryRequest` → `ChatQueryResponse` | Executes a grounded query and returns authorized citations. |
| `GET /api/v1/chat/sessions` | `ChatSessionSummaryResponse[]` | Lists the caller's chat sessions. |

Before a production implementation, each endpoint must require authentication and resolve
the application user from the Entra identity. Chat retrieval must enforce the required
workspace/document/version access predicate described in `database-architecture.md`.
