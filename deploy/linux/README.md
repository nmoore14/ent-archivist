# Linux deployment scaffold

The original project supported offline installation and Nginx. Preserve that operational requirement, but do not copy the prior scripts unchanged: this application will use separate API, worker, and Nuxt web processes.

Before implementing deployment scripts, decide whether the production target is Azure Container Apps, a VM-hosted Compose deployment, or both. Required outcomes remain: HTTPS termination, health checks, restart behavior, log collection, and a documented offline package process.
