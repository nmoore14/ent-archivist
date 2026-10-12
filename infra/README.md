# Infrastructure

`docker/compose.yml` supplies the complete local development stack: Nuxt web, .NET API, .NET worker, and Microsoft SQL Server 2025. API and worker source are mounted and run under `dotnet watch`; the Nuxt source is mounted and uses polling file watching for macOS Docker file sharing.

The development images intentionally contain only the SDK/runtime. `dotnet watch` and the Nuxt startup command restore dependencies after the source mount is active, which makes dependency errors visible in the service logs and avoids stale image-layer restores.

Start it from the repository root:

```sh
docker compose up --build
```

- Web: `http://localhost:3000`
- API: `http://localhost:5080`
- API health: `http://localhost:5080/health`

The API port is configurable with `ARCHIVIST_API_PORT` in a root `.env` file; it defaults to `5080` so it does not collide with a typical local .NET development server on port 5000.

No local .NET installation is required for this workflow. Azure infrastructure, queues, Blob Storage, Key Vault, Application Insights, Entra application registration, and AI Foundry resources have not been provisioned yet.

Use Bicep or Terraform consistently once the target Azure environment is approved.
