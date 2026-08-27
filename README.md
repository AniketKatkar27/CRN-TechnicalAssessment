# CRN Technical Assessment

## RESTful Backend API Solution

A RESTful backend API solution for managing Products and Items, developed as part of the CRN Technologies Technical Assessment.

The solution is designed using a layered architecture with a focus on maintainability, security, testability, and containerized deployment.

---

## 1. Technology Stack

| Component | Technology |
|---|---|
| Framework | .NET 8 |
| Language | C# |
| API | ASP.NET Core Web API |
| Database | Microsoft SQL Server |
| ORM | Entity Framework Core |
| Authentication | ASP.NET Core Identity + JWT Bearer |
| Refresh Tokens | Refresh Token Strategy |
| Validation | FluentValidation |
| Unit Testing | xUnit |
| Mocking | Moq |
| Integration Testing | WebApplicationFactory |
| API Documentation | Swagger / OpenAPI |
| Containerization | Docker |
| Orchestration | Docker Compose |

---

## 2. Solution Architecture

The solution follows a layered architecture:

```text
                         Client
                           |
                           v
                       CRN.API
                           |
                           v
                  CRN.Application
                           |
                           v
                      CRN.Domain
                           ^
                           |
                  CRN.Infrastructure
                     /           \
                    v             v
              SQL Server       Identity
```

### CRN.API

Responsible for:

- HTTP endpoints
- Controllers
- Middleware
- Authentication and authorization configuration
- Swagger/OpenAPI
- CORS
- Response compression
- HTTP request pipeline

### CRN.Application

Contains application-level contracts and logic:

- DTOs
- Interfaces
- Application services
- Validation
- Repository and Unit of Work contracts

### CRN.Domain

Contains core domain entities:

- Product
- Item
- RefreshToken

### CRN.Infrastructure

Contains implementation details:

- Entity Framework Core DbContexts
- Entity configurations
- Repository implementations
- Unit of Work
- ASP.NET Core Identity
- Authentication services
- JWT token services
- Database migrations
- Identity seeding

### CRN.UnitTests

Contains unit tests for application/service logic.

### CRN.IntegrationTests

Contains API integration tests using `WebApplicationFactory`.

---

## 3. Project Structure

```text
CRN.TechnicalAssessment/
│
├── CRN.API/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   └── ProductsController.cs
│   │
│   ├── Middleware/
│   │   ├── ExceptionHandlingMiddleware.cs
│   │   ├── RequestLoggingMiddleware.cs
│   │   └── SecurityHeaderMiddleware.cs
│   │
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Program.cs
│   ├── Dockerfile
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── appsettings.Docker.json
│
├── CRN.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Services/
│   └── Validators/
│
├── CRN.Domain/
│   └── Entities/
│
├── CRN.Infrastructure/
│   ├── Data/
│   ├── Identity/
│   ├── Migrations/
│   └── Services/
│
├── CRN.UnitTests/
│
├── CRN.IntegrationTests/
│
├── docker-compose.yml
├── .gitignore
└── README.md
```

---

# 4. REST API

The application provides RESTful Product APIs.

| Operation | HTTP Method | Endpoint | Tested Status |
|---|---|---|---|
| Get products | GET | `/api/Products` | `200 OK` |
| Get product by ID | GET | `/api/Products/{id}` | `200 OK` |
| Create product | POST | `/api/Products` | `200 OK` |
| Update product | PUT | `/api/Products/{id}` | `204 No Content` |
| Delete product | DELETE | `/api/Products/{id}` | `204 No Content` |

Product collection retrieval supports pagination.

---

# 5. Authentication and Authorization

The API uses:

- ASP.NET Core Identity
- JWT Bearer authentication
- Short-lived access tokens
- Refresh tokens
- Role-based authorization

## Authentication Flow

```text
Register / Login
       |
       v
Validate credentials
       |
       v
Generate access + refresh tokens
       |
       +----------------+
       |                |
       v                v
 Access Token       Refresh Token
       |                |
       v                v
API Requests       Token Refresh
```

For protected requests:

```text
Client
  |
  | Authorization: Bearer <access-token>
  v
JWT Authentication
  |
  v
Authorization / Role Check
  |
  v
Controller
  |
  v
Application Service
  |
  v
Database
```

When the access token expires, the refresh-token endpoint can be used to obtain a new access token according to the implemented refresh-token strategy.

---

# 6. Validation

Incoming request DTOs are validated using FluentValidation.

Validation is performed before invalid request data is processed by the application and persistence layers.

---

# 7. Error Handling

Centralized exception handling is implemented through:

```text
ExceptionHandlingMiddleware
```

The middleware provides consistent handling of unexpected application exceptions and API error responses.

---

# 8. Request Logging

The application contains:

```text
RequestLoggingMiddleware
```

The middleware records relevant request and response information, including:

- HTTP method
- Request path
- Response status
- Request execution duration

---

# 9. Security Headers

Security-related response headers are applied through:

```text
SecurityHeaderMiddleware
```

This provides an additional layer of protection against common browser-based security risks.

---

# 10. CORS

A dedicated CORS policy is configured for the API.

The policy controls permitted origins, headers, and HTTP methods.

---

# 11. Response Compression

Response compression is enabled to reduce HTTP response payload sizes where applicable and improve network efficiency.

---

# 12. Database

The application uses Microsoft SQL Server with Entity Framework Core.

The main application entities are:

```text
Product
   |
   | 1
   |
   | *
 Item
```

`Item.ProductId` is a foreign key referencing `Product.Id`.

### Product

| Column | Type | Nullable |
|---|---|---|
| Id | INT | No |
| ProductName | NVARCHAR(255) | No |
| CreatedBy | NVARCHAR(100) | No |
| CreatedOn | DATETIME2 | No |
| ModifiedBy | NVARCHAR(100) | Yes |
| ModifiedOn | DATETIME2 | Yes |

### Item

| Column | Type | Nullable |
|---|---|---|
| Id | INT | No |
| ProductId | INT | No |
| Quantity | INT | No |

### RefreshToken

The refresh-token table stores token information required by the refresh-token strategy.

ASP.NET Core Identity manages authentication-related tables including users, roles, claims, logins, and user-role relationships.

---

# 13. Entity Framework Core and Migrations

Entity Framework Core is used for database access and schema management.

Migrations are located under:

```text
CRN.Infrastructure/Migrations/
```

The application applies pending migrations during startup.

The migrations create the required Product, Item, RefreshToken, and ASP.NET Core Identity database structures.

---

# 14. Identity Seeding

The application includes an Identity seeder responsible for initializing the required roles and users.

Role-based authorization is used to protect operations requiring authorization.

---

# 15. Performance Considerations

The implementation includes:

- `AsNoTracking()` for appropriate read-only queries
- Pagination for product collections
- Database indexes
- Async/await for database and API operations
- Response compression

Pagination limits the amount of data returned in a single collection response.

---

# 16. Swagger / OpenAPI

Swagger/OpenAPI documentation is provided using Swashbuckle.

When the API is running, Swagger UI is available at:

```text
/swagger
```

### Local example

```text
https://localhost:<port>/swagger
```

### Docker example

```text
http://localhost:7002/swagger
```

Swagger supports Bearer authentication.

To test protected endpoints:

1. Register or log in.
2. Copy the returned access token.
3. Select **Authorize** in Swagger.
4. Provide the JWT bearer token.
5. Execute the protected endpoint.

---

# 17. Running Locally

## Prerequisites

- .NET 8 SDK
- SQL Server or SQL Server Express
- Visual Studio 2022 or later

## Configuration

The application uses environment-specific configuration files.

Non-sensitive configuration is stored in:

```text
CRN.API/appsettings.json
```

Local sensitive configuration should be supplied through ASP.NET Core User Secrets or environment variables.

Required values include:

```text
ConnectionStrings:DefaultConnection
Jwt:Key
```

Sensitive values should not be committed to source control.

## Run the API

Open:

```text
CRN.TechnicalAssessment.slnx
```

in Visual Studio.

Set `CRN.API` as the startup project and run the application.

Swagger will be available at the configured development URL.

---

# 18. Running with Docker

Docker Compose runs the API and SQL Server together.

## Start

From the solution root:

```bash
docker compose up --build
```

## Start in detached mode

```bash
docker compose up --build -d
```

## Check containers

```bash
docker compose ps
```

## View API logs

```bash
docker logs crn-api
```

## Stop

```bash
docker compose down
```

### Docker API

```text
http://localhost:7002
```

### Docker Swagger

```text
http://localhost:7002/swagger
```

### SQL Server

```text
localhost:1433
```

The API container connects to SQL Server using the Docker Compose service name rather than `localhost`.

---

# 19. Docker Environment Configuration

Docker configuration uses environment variables.

Sensitive Docker configuration is supplied through `.env`.

Example:

```text
MSSQL_SA_PASSWORD=<database-password>
JWT_KEY=<jwt-signing-key>
```

The `.env` file is excluded from source control using `.gitignore`.

Secrets should be supplied through the deployment environment or a secure secrets-management solution in production.

---

# 20. Testing

The solution contains unit tests and integration tests.

## Unit Tests

Unit tests are located in:

```text
CRN.UnitTests/
```

Run:

```bash
dotnet test CRN.UnitTests
```

## Integration Tests

Integration tests are located in:

```text
CRN.IntegrationTests/
```

They use `WebApplicationFactory` to test the API through its HTTP pipeline.

Run:

```bash
dotnet test CRN.IntegrationTests
```

## Run All Tests

From the solution root:

```bash
dotnet test
```

The implemented test suites cover Product API behavior and service-level functionality.

---

# 21. Security Measures

The application implements:

- JWT authentication
- Short-lived access tokens
- Refresh token strategy
- Role-based authorization
- FluentValidation
- Centralized exception handling
- CORS policy
- HTTPS support
- Security response headers
- Parameterized database access through Entity Framework Core
- Secrets excluded from source control

---

# 22. Deployment

The application can be deployed using Docker Compose.

Build and start:

```bash
docker compose up --build -d
```

Verify:

```bash
docker compose ps
```

Then access:

```text
http://localhost:7002/swagger
```

For production deployment:

- Use production-specific configuration.
- Provide secrets through environment variables or a secrets-management system.
- Do not commit `.env` or credentials.
- Configure HTTPS/TLS.
- Configure production CORS origins.
- Use a production SQL Server instance.
- Review container and database persistence requirements.

---

# 23. High-Level Deployment Architecture

```text
                         Client
                           |
                           v
                    HTTP / HTTPS
                           |
                           v
                  CRN API Container
                           |
                           v
                 SQL Server Container
                           |
                           v
                    Docker Volume
```

---

# 24. Configuration Environments

## Development

```text
ASPNETCORE_ENVIRONMENT=Development
```

Development configuration is used for local execution.

## Docker

```text
ASPNETCORE_ENVIRONMENT=Docker
```

Docker-specific configuration is supplied through Docker Compose and environment variables.

---

# 25. API Testing Workflow

A typical API workflow is:

```text
1. Register user
       |
       v
2. Login
       |
       v
3. Receive JWT access token
       |
       v
4. Authorize in Swagger
       |
       v
5. Get Products
       |
       v
6. Create Product
       |
       v
7. Get Product by ID
       |
       v
8. Update Product
       |
       v
9. Delete Product
```

---

# 26. Source Control

The repository excludes local and generated files including:

- `.env`
- `.vs/`
- `bin/`
- `obj/`
- User-specific project files

Sensitive configuration values are not stored in the committed application configuration.

---

# 27. Repository

The source code is available in the public GitHub repository:

https://github.com/AniketKatkar27/CRN-TechnicalAssessment

---

# 28. Assessment Coverage

The implementation covers the requested technical areas including:

- RESTful Product API
- .NET 8 / ASP.NET Core
- Layered architecture
- SQL Server
- Entity Framework Core
- JWT authentication
- Refresh tokens
- Role-based authorization
- FluentValidation
- Repository pattern
- Unit of Work
- Service layer
- Centralized exception handling
- Request logging
- Security headers
- CORS
- Response compression
- Async/await
- Pagination
- Database indexing
- Unit testing
- Integration testing
- Swagger/OpenAPI
- Docker
- Docker Compose
- Environment-based configuration
- EF Core migrations
- Identity seeding
