# SHC DRIVE

SHC DRIVE is a secure cloud storage workspace with a .NET 8 API, a Vite React frontend, and a lightweight storage-node service for storing uploaded files on self-hosted machines.

## What It Does

- User registration, login, JWT auth, and role-aware dashboards
- File and folder upload, download, rename, move, delete, trash, and restore
- Share links for files and folders
- Admin management for users, roles, permissions, storage nodes, subscriptions, purchases, and system settings
- Notifications, audit logs, AI suggestions, and Stripe Checkout support

## Tech Stack

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core with MySQL
- Clean architecture style: `domain`, `application`, `adapters`
- React 18 + Vite
- Stripe.net for payments

## Project Structure

```text
domain/                         Domain entities and enums
application/                    Use cases, DTOs, and ports
adapters/Driven/                Persistence and external services
adapters/Driving/Api/           Main backend API
adapters/Driving/StorageNode/   Storage node service
frontend/                       React frontend
```

## Requirements

- .NET SDK 8
- Node.js 18+
- MySQL 8+
- Optional: Stripe CLI for local webhook testing

## Setup

Configure the API with your own local values. Prefer user secrets for sensitive data:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "server=localhost;port=3306;database=shc;user=root;password=YOUR_PASSWORD;SslMode=None;AllowPublicKeyRetrieval=True" --project adapters/Driving/Api/api.csproj
dotnet user-secrets set "Jwt:Key" "YOUR_LONG_RANDOM_SIGNING_KEY" --project adapters/Driving/Api/api.csproj
dotnet user-secrets set "Jwt:Issuer" "SHC" --project adapters/Driving/Api/api.csproj
dotnet user-secrets set "Jwt:Audience" "SHC.Client" --project adapters/Driving/Api/api.csproj
dotnet user-secrets set "Stripe:SecretKey" "sk_test_..." --project adapters/Driving/Api/api.csproj
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_..." --project adapters/Driving/Api/api.csproj
```

Apply database migrations:

```powershell
dotnet ef database update --project adapters/adapters.csproj --startup-project adapters/Driving/Api/api.csproj --context ShcDbContext
```

Install frontend dependencies:

```powershell
cd frontend
npm install
```

## Run Locally

Start the API:

```powershell
dotnet run --project adapters/Driving/Api/api.csproj --launch-profile http
```

API URL: `http://localhost:5165`  
Swagger: `http://localhost:5165/swagger`

Start the storage node:

```powershell
dotnet run --project adapters/Driving/StorageNode/StorageNode.csproj
```

Storage node URL: `http://localhost:7001`

Start the frontend:

```powershell
cd frontend
npm run dev
```

Frontend URL: `http://127.0.0.1:5173`

## Useful Commands

```powershell
dotnet build SHC.sln
dotnet run --project adapters/Driving/Api/api.csproj --launch-profile http
dotnet run --project adapters/Driving/StorageNode/StorageNode.csproj
cd frontend
npm run build
```

## API Areas

- `/api/auth`
- `/api/users`
- `/api/files`
- `/api/folders`
- `/api/shared-links`
- `/api/storage-nodes`
- `/api/subscriptions`
- `/api/purchases`
- `/api/permissions`
- `/api/roles`
- `/api/notifications`
- `/api/audit-logs`

## Stripe

Stripe Checkout is documented in [STRIPE_INTEGRATION.md](STRIPE_INTEGRATION.md).

For local webhook testing:

```powershell
stripe listen --forward-to localhost:5165/api/webhooks/stripe
```

## Notes

- CORS is configured for the Vite dev server on ports `5173` and `5175`.
- Keep real database passwords, JWT keys, and Stripe secrets out of committed files.
