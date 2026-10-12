# Decisions and open questions

## Decided

- Frontend: Nuxt 4, Vue 3, TypeScript.
- Backend: ASP.NET Core / .NET 10 with a separate worker.
- Initial system of record and retrieval store: Microsoft SQL Server 2025. Native
  vector storage and exact vector-distance search are used for v1; approximate
  vector indexes are not a v1 dependency because they remain preview in SQL Server.
- Identity: Microsoft Entra ID.
- Authorization: roles and permissions controlled in the application database.
- User experience: one global chat; content is selected solely by effective workspace permissions.
- Operational parity goal: Docker Compose and offline Linux/Nginx deployment remain supported requirements.
- Tenancy: v1 is single-tenant. Application records do not carry a tenant or organization
  foreign key. Tenant isolation, should it be required later, is a deliberate schema
  migration rather than an implied behavior.

## Not yet decided

- Azure queue technology and Blob/Foundry provisioning approach.
- Exact content-job connectors, fetch policy, and approval/review flow.
- Chat-prompt retention, redaction, export, and deletion policy.
- OCR support for scanned PDFs.
- Whether direct user grants are sufficient for v1 or Entra group mappings are needed immediately.
