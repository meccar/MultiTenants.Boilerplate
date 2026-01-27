# Multi-Tenant Boilerplate

A complete multi-tenant DDD (Domain-Driven Design) layered architecture project built with .NET 10.0.

## Architecture Overview

This project follows a clean layered architecture:

```
┌─────────────────┐
│   HttpApi       │  ← Carter endpoints, Swagger, OAuth
├─────────────────┤
│   Application   │  ← CQRS with MediatR, MongoDB stores
├─────────────────┤
│   Domain        │  ← Entities, Value Objects, Domain Events
├─────────────────┤
│   Shared        │  ← Constants, Utilities
└─────────────────┘
```

## Features

- ✅ **Multi-Tenancy**: Full multi-tenant support using Finbuckle.MultiTenant
- ✅ **Database**: MongoDB for primary data storage
- ✅ **Document Store**: Marten for event sourcing/document store
- ✅ **Authentication**: ASP.NET Identity with MongoDB stores
- ✅ **OAuth**: Gmail OAuth integration
- ✅ **CQRS**: Command Query Responsibility Segregation with MediatR
- ✅ **API Framework**: Carter for building HTTP APIs (no controllers)
- ✅ **Documentation**: Swagger/OpenAPI integration
- ✅ **DDD**: Domain-Driven Design principles

## Project Structure

```
MultiTenants.Boilerplate/
├── MultiTenants.Boilerplate.Shared/      # Shared constants and utilities
├── MultiTenants.Boilerplate.Domain/     # Domain entities, value objects
├── MultiTenants.Boilerplate.Application/ # CQRS commands, queries, handlers
└── MultiTenants.Boilerplate.HttpApi/    # Carter endpoints, API configuration
```

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- MongoDB (running locally or via Docker)
- PostgreSQL (for Marten)
- Google OAuth credentials

### Configuration

1. Update `appsettings.json` or `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017",
    "PostgreSQL": "Host=localhost;Port=5432;Database=multitenants;Username=postgres;Password=postgres"
  },
  "MongoDB": {
    "DatabaseName": "multitenants"
  },
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  }
}
```

2. Set up Google OAuth:
   - Go to [Google Cloud Console](https://console.cloud.google.com/)
   - Create OAuth 2.0 credentials
   - Add redirect URI: `https://localhost:5001/api/auth/login/google/callback`
   - Update appsettings with ClientId and ClientSecret

### Running with Docker

```bash
docker-compose up -d
```

This will start:
- MongoDB on port 27017
- PostgreSQL on port 5432
- API on ports 5000 (HTTP) and 5001 (HTTPS)

### Running Locally

```bash
cd MultiTenants.Boilerplate.HttpApi
dotnet run
```

Access Swagger UI at: `https://localhost:5001/swagger`

## API Endpoints

### Authentication
- `GET /api/auth/login/google` - Initiate Google OAuth
- `GET /api/auth/login/google/callback` - OAuth callback
- `POST /api/auth/logout` - Logout
- `GET /api/auth/me` - Get current user

### Users
- `POST /api/users` - Create user
- `GET /api/users/{id}` - Get user by ID

## Multi-Tenancy

The API uses route-based tenant identification:

```
/{tenant}/api/users
```

Example: `/tenant1/api/users`

## Technologies Used

- **.NET 10.0**: Latest .NET framework
- **Finbuckle.MultiTenant**: Multi-tenancy support
- **MongoDB**: NoSQL database
- **Marten**: Document database and event store
- **ASP.NET Identity**: Authentication and authorization
- **MediatR**: CQRS implementation
- **Carter**: Minimal API framework
- **Swagger**: API documentation

## License

This is a boilerplate project for building multi-tenant applications.

