# SHC DRIVE

SHC DRIVE is a secure cloud workspace for storing, organizing, sharing, and managing files across a self-hosted storage-node network. The repository contains a .NET 8 backend built with clean architecture, a small storage-node service, and a Vite + React frontend for user and admin workflows.

## Features

- JWT-based registration, login, logout, refresh-token storage, and role-aware dashboard routing
- File and folder management with upload, download, rename, move, archive, delete, trash, and restore flows
- Distributed storage-node support through a separate lightweight storage node API
- Link sharing for files and folders with permissions and deactivation
- Role, permission, ownership, and audit-log modules
- Notifications and AI suggestion workflows
- Subscription plans, purchases, invoices, and Stripe Checkout integration
- Admin dashboard for users, roles, storage nodes, subscriptions, purchases, system settings, and audit logs
- User dashboard for files, shares, notifications, subscription management, profile, settings, and trash

## Tech Stack

- Backend: .NET 8, ASP.NET Core Web API
- Architecture: domain, application, adapters, and driving adapter projects
- Persistence: Entity Framework Core 8 with MySQL via Pomelo
- Optional document data: MongoDB driver is referenced by the API/adapters layer
- Authentication: JWT bearer authentication and refresh tokens
- Payments: Stripe.net
- API docs: Swagger / Swashbuckle in development
- Frontend: React 18, Vite 4

## Repository Layout

```text
.
|-- domain/                         # Domain entities and enums
|-- application/                    # Use cases, DTOs, and ports
|-- adapters/
|   |-- Driven/                     # Persistence and external service adapters
|   `-- Driving/
|       |-- Api/                    # Main ASP.NET Core API
|       `-- StorageNode/            # Storage node HTTP service
|-- frontend/                       # Vite React client
|-- STRIPE_INTEGRATION.md           # Detailed Stripe integration notes
|-- SHC.sln                         # .NET solution
`-- global.json                     # Pins .NET SDK 8.0.416
```

## Prerequisites

- .NET SDK 8.0.416 or compatible .NET 8 SDK
- Node.js 18+ and npm
- MySQL 8.0+
- Optional: MongoDB, if you enable features that use the configured Mongo connection
- Optional: Stripe account and Stripe CLI for local payment webhook testing

## Configuration

The main API reads configuration from `adapters/Driving/Api/appsettings.json`, environment-specific appsettings files, environment variables, and .NET user secrets.

For a public GitHub repository, keep real secrets out of committed files. Prefer user secrets locally:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=shc;user=root;password=YOUR_PASSWORD;SslMode=None;AllowPublicKeyRetrieval=True" --project adapters/Driving/Api/api.csproj
dotnet user-secrets set "Jwt:Key" "YOUR_LONG_RANDOM_SIGNING_KEY" --project adapters/Driving/Api/api.csproj
dotnet user-secrets set "Jwt:Issuer" "SHC" --project adapters/Driving/Api/api.csproj
dotnet user-secrets set "Jwt:Audience" "SHC.Client" --project adapters/Driving/Api/api.csproj
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..." --project adapters/Driving/Api/api.csproj
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_..." --project adapters/Driving/Api/api.csproj
dotnet user-secrets set "StorageNodes:ApiKey" "YOUR_STORAGE_NODE_SHARED_KEY" --project adapters/Driving/Api/api.csproj
```

Useful configuration keys:

| Key | Purpose |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | MySQL database connection used by `ShcDbContext` |
| `MongoDB:ConnectionString` | MongoDB server URL |
| `MongoDB:Database` | MongoDB database name |
| `Jwt:Key` | JWT signing key |
| `Jwt:Issuer` | JWT issuer |
| `Jwt:Audience` | JWT audience |
| `Jwt:ExpiryMinutes` | Access-token lifetime |
| `Stripe:SecretKey` | Stripe API secret key |
| `Stripe:WebhookSecret` | Stripe webhook signing secret |
| `StorageNodes:ApiKey` | Shared key the main API sends to storage nodes |

The storage node service reads:

| Key | Purpose |
| --- | --- |
| `StorageNode:BasePath` | Local directory where uploaded file bytes are stored |
| `StorageNode:ApiKey` | Shared key expected in `X-Storage-Node-Key`; if omitted, the node accepts requests without the key |

## Database Setup

Create a MySQL database named `shc` or update `ConnectionStrings:DefaultConnection` to point to your preferred database.

Apply the EF Core migrations from the repository root:

```powershell
dotnet ef database update --project adapters/adapters.csproj --startup-project adapters/Driving/Api/api.csproj --context ShcDbContext
```

If `dotnet ef` is not installed:

```powershell
dotnet tool install --global dotnet-ef
```

## Running Locally

Install frontend dependencies:

```powershell
cd frontend
npm install
```

Run the main API from the repository root:

```powershell
dotnet run --project adapters/Driving/Api/api.csproj --launch-profile http
```

The API starts at:

- `http://localhost:5165`
- Swagger UI: `http://localhost:5165/swagger`

Run a storage node in a second terminal:

```powershell
dotnet run --project adapters/Driving/StorageNode/StorageNode.csproj
```

The storage node listens on:

- `http://localhost:7001`
- Health check: `GET http://localhost:7001/api/node/health`

Run the frontend in a third terminal:

```powershell
cd frontend
npm run dev
```

The Vite app starts at:

- `http://127.0.0.1:5173`

The frontend defaults to `http://127.0.0.1:5165` for API calls. You can override it in browser local storage with the `shc.apiUrl` key.

## Storage Node Flow

1. Start one or more storage node services.
2. In the admin dashboard, register each node with its host, port, base path, and capacity.
3. When a user uploads a file, the API allocates a storage node and sends the bytes to `/api/node/files`.
4. Downloads and deletes route back through the registered node using the stored path.

The node service protects file paths by resolving every stored path under `StorageNode:BasePath`.

## Stripe Flow

Stripe support is isolated behind the payment gateway port in the application layer and implemented in `adapters/Driven/ExternalServices/Payments`.

Local webhook testing example:

```powershell
stripe login
stripe listen --forward-to localhost:5165/api/webhooks/stripe
```

Set the printed webhook secret as `Stripe:WebhookSecret`.

See [STRIPE_INTEGRATION.md](STRIPE_INTEGRATION.md) for a deeper breakdown of the payment architecture and handled webhook events.

## API Areas

The main API exposes these route groups:

| Area | Base route |
| --- | --- |
| Auth | `/api/auth` |
| Users and settings | `/api/users` |
| Files | `/api/files` |
| Folders | `/api/folders` |
| Shared links | `/api/shared-links` |
| Storage nodes | `/api/storage-nodes` |
| Trash | `/api/trash` |
| Notifications | `/api/notifications` |
| AI suggestions | `/api/ai-suggestions` |
| Roles | `/api/roles` |
| Permissions | `/api/permissions` |
| Audit logs | `/api/audit-logs` |
| System settings | `/api/system-settings` |
| Purchases and checkout | `/api/purchases` |
| Subscriptions | `/api/subscriptions` |
| Webhooks | `/api/webhooks` |

Use Swagger in development for request and response details.

## Common Commands

Build the .NET solution:

```powershell
dotnet build SHC.sln
```

Run the API:

```powershell
dotnet run --project adapters/Driving/Api/api.csproj --launch-profile http
```

Run the storage node:

```powershell
dotnet run --project adapters/Driving/StorageNode/StorageNode.csproj
```

Build the frontend:

```powershell
cd frontend
npm run build
```

Preview the frontend build:

```powershell
cd frontend
npm run preview
```

## Development Notes

- CORS is configured for Vite on `localhost` and `127.0.0.1` ports `5173` and `5175`.
- Swagger is enabled only when `ASPNETCORE_ENVIRONMENT=Development`.
- HTTPS redirection is enabled outside development.
- The project currently does not include a test project. Add focused tests around high-risk use cases such as auth, permissions, file routing, billing status changes, and storage-node path handling.
- Review committed configuration before publishing the repository. Real database passwords, JWT keys, Stripe keys, and webhook secrets should be rotated if they were ever committed.
