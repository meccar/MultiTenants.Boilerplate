# Multi-Tenant Boilerplate API

This is a multi-tenant DDD layered project with the following architecture:

## Architecture

- **Domain Layer**: Entities, Value Objects, Domain Events
- **Application Layer**: CQRS with MediatR (Commands and Queries)
- **HttpApi Layer**: Carter endpoints that use Application layer handlers
- **Shared Layer**: Constants and utilities

## Features

- ✅ Multi-tenancy with Finbuckle.MultiTenant
- ✅ MongoDB for data storage
- ✅ Marten for event sourcing/document store
- ✅ ASP.NET Identity with MongoDB stores
- ✅ Gmail OAuth authentication
- ✅ CQRS pattern with MediatR
- ✅ Carter for HTTP API endpoints
- ✅ Swagger for API documentation

## Setup

### Prerequisites

1. MongoDB running on `localhost:27017`
2. PostgreSQL running (for Marten)
3. Google OAuth credentials

### Configuration

Sensitive configuration values (connection strings and OAuth credentials) should be set via environment variables to avoid exposing them in configuration files. The application will automatically load these from environment variables, which override values in `appsettings.json`.

#### Required Environment Variables

Set the following environment variables:

**Connection Strings:**
- `ConnectionStrings__MongoDB` - MongoDB connection string (e.g., `mongodb://localhost:27017`)
- `ConnectionStrings__PostgreSQL` - PostgreSQL connection string (e.g., `Host=localhost;Port=5432;Database=multitenants;Username=postgres;Password=postgres`)

**Google OAuth:**
- `Authentication__Google__ClientId` - Google OAuth Client ID
- `Authentication__Google__ClientSecret` - Google OAuth Client Secret

**MongoDB Database Name:**
- `MongoDB__DatabaseName` - Database name (defaults to `multitenants` if not set)

#### Setting Environment Variables

**Linux/macOS:**
```bash
export ConnectionStrings__MongoDB="mongodb://localhost:27017"
export ConnectionStrings__PostgreSQL="Host=localhost;Port=5432;Database=multitenants;Username=postgres;Password=postgres"
export Authentication__Google__ClientId="your-client-id"
export Authentication__Google__ClientSecret="your-client-secret"
```

**Windows (PowerShell):**
```powershell
$env:ConnectionStrings__MongoDB="mongodb://localhost:27017"
$env:ConnectionStrings__PostgreSQL="Host=localhost;Port=5432;Database=multitenants;Username=postgres;Password=postgres"
$env:Authentication__Google__ClientId="your-client-id"
$env:Authentication__Google__ClientSecret="your-client-secret"
```

**Using .env file (with dotnet user-secrets or similar):**
Alternatively, you can use ASP.NET Core User Secrets for development:
```bash
dotnet user-secrets set "ConnectionStrings:MongoDB" "mongodb://localhost:27017"
dotnet user-secrets set "ConnectionStrings:PostgreSQL" "Host=localhost;Port=5432;Database=multitenants;Username=postgres;Password=postgres"
dotnet user-secrets set "Authentication:Google:ClientId" "your-client-id"
dotnet user-secrets set "Authentication:Google:ClientSecret" "your-client-secret"
```

### Google OAuth Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project or select an existing one
3. Enable Google+ API
4. Create OAuth 2.0 credentials
5. Add authorized redirect URI: `https://localhost:5001/api/auth/login/google/callback`
6. Copy Client ID and Client Secret to appsettings

## API Endpoints

### Authentication
- `GET /api/auth/login/google` - Initiate Google OAuth login
- `GET /api/auth/login/google/callback` - Google OAuth callback
- `POST /api/auth/logout` - Logout current user
- `GET /api/auth/me` - Get current authenticated user

### Users
- `POST /api/users` - Create a new user
- `GET /api/users/{id}` - Get user by ID

## Multi-Tenancy

The API uses route-based tenant identification. Include the tenant identifier in the route:

```
/{tenant}/api/users
```

Example:
```
/tenant1/api/users
```

## Swagger

Access Swagger UI at: `https://localhost:5001/swagger` (in development mode)

