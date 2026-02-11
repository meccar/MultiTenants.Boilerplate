# Multi-Tenant Boilerplate

A complete multi-tenant DDD (Domain-Driven Design) layered architecture project built with .NET 10.0.

## Architecture Overview

This project follows a clean layered architecture with **layer-by-layer dependency injection**:

```
┌─────────────────────────────────┐
│   HttpApi (Outer Layer)          │  ← Orchestrates all layers
│   - Carter endpoints            │  ← Swagger, OAuth, Multi-tenant
│   - API configuration           │  ← CORS, Rate Limiting, Health Checks
├─────────────────────────────────┤
│   Application Layer             │  ← Business logic, CQRS, services
│   - CQRS (MediatR)              │  ← Commands, Queries, Handlers
│   - Validation (FluentValidation)│  ← Automatic validation pipeline
│   - Databases (MongoDB, Marten) │  ← Data stores
│   - Message Broker (RabbitMQ/Kafka)│ ← Optional messaging
│   - Caching (Redis/Memory)     │  ← Optional caching
├─────────────────────────────────┤
│   Domain Layer                  │  ← Entities, Value Objects
│   - Domain entities             │  ← Business domain models
│   - Domain logic                │  ← Core business rules
├─────────────────────────────────┤
│   Shared Layer (Foundation)     │  ← Constants, Utilities
│   - Constants                   │  ← API, Auth, Validation constants
│   - Utilities                   │  ← Helpers, Extensions
│   - Responses                   │  ← API response models
└─────────────────────────────────┘
```

## Features

### Core Features
- ✅ **Multi-Tenancy**: Full multi-tenant support using Finbuckle.MultiTenant
- ✅ **Database**: MongoDB for primary data storage
- ✅ **Document Store**: Marten for event sourcing/document store
- ✅ **Authentication**: ASP.NET Identity with MongoDB stores
- ✅ **OAuth**: Google OAuth integration
- ✅ **CQRS**: Command Query Responsibility Segregation with MediatR
- ✅ **API Framework**: Carter for building HTTP APIs (no controllers)
- ✅ **Documentation**: Swagger/OpenAPI integration
- ✅ **DDD**: Domain-Driven Design principles

### Quality & Reliability
- ✅ **Validation**: FluentValidation with automatic pipeline validation
- ✅ **Health Checks**: MongoDB and PostgreSQL health check endpoints (DI-driven)
- ✅ **Rate Limiting**: Configurable rate limiting with sliding window
- ✅ **Exception Handling**: Global exception handling middleware
- ✅ **CORS**: Configurable CORS policies
- ✅ **Response Wrapper**: Standardized API response format

### Optional Features (DI-Driven)
- ✅ **Message Broker**: RabbitMQ or Kafka support (configured via appsettings)
- ✅ **Caching**: Redis (distributed) or In-Memory caching (configured via appsettings)

## Project Structure

```
MultiTenants.Boilerplate/
├── MultiTenants.Boilerplate.Shared/           # Shared constants and utilities
│   ├── Constants/                            # API, Auth, Validation constants
│   ├── Responses/                            # API response models
│   ├── Utilities/                             # Helpers, extensions
│   └── Configuration/                       # SharedConfiguration.cs (DI entry)
│
├── MultiTenants.Boilerplate.Domain/          # Domain entities, value objects
│   ├── Entities/                             # Domain entities
│   └── Configuration/                        # DomainConfiguration.cs (DI entry)
│
├── MultiTenants.Boilerplate.Application/      # CQRS commands, queries, handlers
│   ├── Commands/                             # Command handlers
│   ├── Queries/                              # Query handlers
│   ├── Validators/                           # FluentValidation validators
│   ├── Messaging/                            # Message broker interfaces
│   │   ├── RabbitMQ/                         # RabbitMQ implementation
│   │   └── Kafka/                            # Kafka implementation
│   ├── Configuration/                        # Service configurations
│   │   ├── ApplicationConfiguration.cs       # Main DI orchestrator
│   │   ├── CQRSConfiguration.cs             # MediatR & Validation
│   │   ├── MongoDbConfiguration.cs           # MongoDB
│   │   ├── PostgreSQLConfiguration.cs       # Marten/PostgreSQL
│   │   ├── IdentityConfiguration.cs         # ASP.NET Identity
│   │   ├── MessageBrokerConfiguration.cs     # RabbitMQ/Kafka
│   │   └── CachingConfiguration.cs           # Redis/In-Memory
│   └── Stores/                               # MongoDB stores
│
└── MultiTenants.Boilerplate.HttpApi/        # Carter endpoints, API configuration
    ├── Endpoints/                            # Carter endpoint modules
    ├── Middlewares/                          # Custom middlewares
    └── Configurations/                       # HttpApi service configurations
        ├── HttpApiConfiguration.cs           # Main DI orchestrator
        ├── SwaggerConfiguration.cs           # Swagger/OpenAPI
        ├── OAuthConfiguration.cs             # Google OAuth
        ├── MultiTenantConfiguration.cs        # Multi-tenancy
        ├── CorsConfiguration.cs              # CORS
        ├── RateLimitingConfiguration.cs      # Rate limiting
        └── HealthCheckConfiguration.cs       # Health checks
```

### Dependency Injection Pattern

Each layer has its own DI entry point, orchestrated by the outer layer:

```csharp
// In Program.cs (HttpApi layer)
builder.Services.AddShared();                    // 1. Shared Layer
builder.Services.AddDomain();                     // 2. Domain Layer
builder.Services.AddApplication(builder.Configuration); // 3. Application Layer
builder.Services.AddHttpApi(builder.Configuration);     // 4. HttpApi Layer
```

## Getting Started

### Prerequisites

- .NET 10.0 SDK
- MongoDB (running locally or via Docker)
- PostgreSQL (for Marten)
- Google OAuth credentials

### Configuration

1. Copy `appsettings.template.json` to `appsettings.Development.json` or update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MongoDB": "mongodb://localhost:27017",
    "PostgreSQL": "Host=localhost;Port=5432;Database=multitenants;Username=postgres;Password=postgres",
    "RabbitMQ": "amqp://guest:guest@localhost:5672/",
    "Kafka": "localhost:9092",
    "Redis": "localhost:6379"
  },
  "MongoDB": {
    "DatabaseName": "multitenants"
  },
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  },
  "MessageBroker": {
    "Type": "rabbitmq",
    "RabbitMQ": {
      "ConnectionString": "amqp://guest:guest@localhost:5672/"
    },
    "Kafka": {
      "BootstrapServers": "localhost:9092",
      "ConsumerGroup": "default-group"
    }
  },
  "Caching": {
    "Type": "memory",
    "Redis": {
      "ConnectionString": "localhost:6379",
      "InstanceName": "MultiTenants:"
    }
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173"
    ]
  },
  "RateLimiting": {
    "PermitLimit": 100,
    "WindowSizeSeconds": 60,
    "QueueLimit": 2
  }
}
```

2. Set up Google OAuth:
   - Go to [Google Cloud Console](https://console.cloud.google.com/)
   - Create OAuth 2.0 credentials
   - Add redirect URI: `https://localhost:5001/api/auth/login/google/callback`
   - Update appsettings with ClientId and ClientSecret
   - Or use User Secrets: `dotnet user-secrets set "Authentication:Google:ClientId" "your-client-id"`

3. **Optional**: Configure Message Broker and Caching:
   - **Message Broker**: Set `MessageBroker:Type` to `"rabbitmq"` or `"kafka"` (or omit to disable)
   - **Caching**: Set `Caching:Type` to `"redis"` for distributed cache, or `"memory"` (default) for in-memory

### Running with Docker

```bash
docker-compose up -d
```

This will start:
- **MongoDB** on port 27017
- **PostgreSQL** on port 5432
- **RabbitMQ** on ports 5672 (AMQP) and 15672 (Management UI)
- **Kafka** on port 9092 (with Zookeeper)
- **Redis** on port 6379
- **API** on ports 5000 (HTTP) and 5001 (HTTPS)

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

### Health Checks
- `GET /health` - Basic health check
- `GET /health/ready` - Readiness check (includes database connections)
- `GET /health/live` - Liveness check

## Multi-Tenancy

The API uses route-based tenant identification:

```
/{tenant}/api/users
```

Example: `/tenant1/api/users`

## Technologies Used

### Core Framework
- **.NET 10.0**: Latest .NET framework
- **Carter**: Minimal API framework (no controllers)
- **MediatR**: CQRS implementation
- **FluentValidation**: Request validation

### Database & Storage
- **MongoDB**: NoSQL database for primary storage
- **Marten**: Document database and event store (PostgreSQL)
- **ASP.NET Identity**: Authentication and authorization with MongoDB stores

### Infrastructure
- **Finbuckle.MultiTenant**: Multi-tenancy support
- **RabbitMQ.Client**: RabbitMQ message broker (optional)
- **Confluent.Kafka**: Kafka message broker (optional)
- **StackExchange.Redis**: Redis distributed cache (optional)
- **Microsoft.Extensions.Caching**: In-memory caching (default)

### API & Documentation
- **Swagger/OpenAPI**: API documentation
- **Google OAuth**: Authentication provider

## Documentation

- **[DI_REGISTRATION_PATTERN.md](DI_REGISTRATION_PATTERN.md)** - Dependency injection pattern and layer structure
- **[SERVICE_REGISTRATION_GUIDE.md](SERVICE_REGISTRATION_GUIDE.md)** - Message broker and caching usage guide
- **[MESSAGE_BROKER_AND_CACHING_SUMMARY.md](MESSAGE_BROKER_AND_CACHING_SUMMARY.md)** - Implementation summary

## Key Design Patterns

- **Clean Architecture**: Layered architecture with clear boundaries
- **CQRS**: Command Query Responsibility Segregation
- **DDD**: Domain-Driven Design principles
- **DI-Driven Configuration**: Services registered based on configuration
- **Unit of Work**: Each layer orchestrates its own services
- **Repository Pattern**: Data access abstraction

## License

This is a boilerplate project for building multi-tenant applications.

