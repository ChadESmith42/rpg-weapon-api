# Weapon API

A comprehensive .NET 9 Web API for weapon management built with Clean Architecture principles, featuring JWT authentication, CRUD operations, and advanced weapon mechanics.

## Features

### Core Functionality

- **JWT Authentication** - Secure user registration, login, and token management
- **Weapon Management** - Complete CRUD operations for weapon entities
- **Weapon Mechanics** - Damage application and repair systems
- **Repair Estimation** - Cost and benefit analysis for weapon repairs
- **Random Generation** - Procedural weapon creation for testing

### Technical Architecture

- **Clean Architecture** - Domain-driven design with clear separation of concerns
- **CQRS Pattern** - Command Query Responsibility Segregation with MediatR
- **Entity Framework Core** - Code-first database approach with PostgreSQL
- **JWT Security** - Comprehensive authentication and authorization
- **Comprehensive Testing** - Unit, integration, and API testing

## Project Structure

```
src/
├── WeaponApi.Domain/          # Core business logic and entities
├── WeaponApi.Application/     # Use cases, commands, queries, and interfaces
├── WeaponApi.Infrastructure/  # External services, database, authentication
└── WeaponApi.Api/            # RESTful API endpoints and controllers

tests/
├── WeaponApi.UnitTests/      # Domain and application layer tests
└── WeaponApi.IntegrationTests/ # API and infrastructure tests

postman/                      # Comprehensive API testing collection
├── WeaponAPI-Comprehensive-Testing.postman_collection.json
├── WeaponAPI-Testing.postman_environment.json
├── setup.sh                  # Automated testing setup
└── README.md                 # Detailed testing documentation
```

## Quick Start

### Prerequisites

- **.NET 9.0 SDK** or later
- **Docker** (for PostgreSQL database)
- **Postman** (optional, for API testing)

### Setup & Run

1. **Clone the repository**:

   ```bash
   git clone <repository-url>
   cd weapon-api
   ```

2. **Start the database**:

   ```bash
   docker-compose up -d postgres
   ```

3. **Run the application**:

   ```bash
   dotnet run --project WeaponApi.Api
   ```

4. **Access the API**:
   - **API Base**: <https://localhost:5001>
   - **Swagger UI**: <https://localhost:5001/swagger>
   - **Health Check**: <https://localhost:5001/health>

## API Testing with Postman

We provide a comprehensive Postman testing collection with **35 requests** covering all API endpoints:

### Quick Test Setup

```bash
# Automated setup (recommended)
./postman/setup.sh
```

This will start the database, build the API, and provide Postman import instructions.

### Manual Test Setup

1. **Import Collection**: `postman/WeaponAPI-Comprehensive-Testing.postman_collection.json`
2. **Import Environment**: `postman/WeaponAPI-Testing.postman_environment.json`
3. **Select Environment**: "Weapon API Testing"
4. **Run Tests**: Execute folders sequentially for best results

### Test Coverage

- **Authentication Tests** (4 requests) - Registration, login, profile, token refresh
- **Weapons CRUD** (4 requests) - Create, read, update, delete operations
- **Advanced Operations** (6 requests) - Damage, repair, estimation workflows
- **Integration Scenarios** (8 requests) - End-to-end user journeys
- **Error Handling** (4 requests) - Validation and error response testing
- **Cleanup Operations** (5 requests) - Test data management

**Result: 100% API endpoint coverage with automated test data management**

**Detailed Testing Guide**: [docs/POSTMAN_TESTING.md](docs/POSTMAN_TESTING.md)

## API Endpoints

### Authentication

- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - User authentication
- `GET /api/auth/profile` - Get user profile (requires auth)
- `POST /api/auth/refresh` - Refresh JWT tokens
- `POST /api/auth/logout` - User logout (requires auth)

### Weapons Management

- `GET /api/weapons` - Get all weapons (requires auth)
- `POST /api/weapons` - Create new weapon (requires auth)
- `GET /api/weapons/{id}` - Get weapon by ID (requires auth)
- `DELETE /api/weapons/{id}` - Delete weapon (requires auth)
- `GET /api/weapons/create-random` - Generate random weapon (requires auth)

### Weapon Operations

- `POST /api/weapons/{id}/damage` - Apply damage to weapon (requires auth)
- `POST /api/weapons/{id}/repair` - Repair weapon (requires auth)
- `POST /api/weapons/estimate-repair` - Get repair cost estimate (requires auth)

## Architecture

### Clean Architecture Layers

**Domain Layer** (`WeaponApi.Domain`)

- Core business entities (User, Weapon)
- Domain events and value objects
- Business rules and invariants

**Application Layer** (`WeaponApi.Application`)

- Use cases (Commands and Queries)
- DTOs and interfaces
- Business logic orchestration

**Infrastructure Layer** (`WeaponApi.Infrastructure`)

- Database context and repositories
- External service implementations
- Authentication services

**API Layer** (`WeaponApi.Api`)

- RESTful controllers
- Request/response models
- Middleware and filters

### Key Patterns

- **CQRS** - Separate read and write operations
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Loose coupling and testability
- **JWT Authentication** - Stateless security
- **Entity Framework Core** - Code-first database approach

## Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/WeaponApi.UnitTests
dotnet test tests/WeaponApi.IntegrationTests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Structure

- **Unit Tests** - Domain and application logic
- **Integration Tests** - API endpoints and database
- **Architecture Tests** - Clean Architecture compliance
- **Postman Tests** - Complete API workflow validation

## Deployment

### Development

```bash
dotnet run --project WeaponApi.Api --configuration Development
```

### Production Build

```bash
dotnet publish WeaponApi.Api -c Release -o publish
```

### Docker Support

```bash
# Build and run with Docker Compose
docker-compose up --build
```

## Configuration

### Database

- **Development**: PostgreSQL via Docker Compose
- **Connection String**: Configured in `appsettings.json`
- **Migrations**: Automatic on startup

### JWT Authentication

```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key",
    "Issuer": "WeaponApi",
    "Audience": "WeaponApiUsers",
    "AccessTokenExpirationInMinutes": 180,
    "RefreshTokenExpirationInDays": 7
  }
}
```

### Environment Variables

- `ASPNETCORE_ENVIRONMENT` - Development/Production
- `ConnectionStrings__DefaultConnection` - Database connection
- `JwtSettings__SecretKey` - JWT signing key

## Documentation

- **Setup Instructions**: [SETUP_INSTRUCTIONS.md](SETUP_INSTRUCTIONS.md)
- **API Testing Guide**: [docs/POSTMAN_TESTING.md](docs/POSTMAN_TESTING.md)
- **Architecture Guidelines**: [.github/instructions/](.\github\instructions\)
- **API Documentation**: Available at `/swagger` when running

## Contributing

1. Follow Clean Architecture principles
2. Ensure comprehensive test coverage
3. Update API tests for new endpoints
4. Follow C# coding standards
5. Run the full test suite before submitting

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## Ready to Build

Your Weapon API development environment is ready. Start with the Postman tests to explore the API, then dive into the codebase to build amazing weapon management features!

For questions or support, check the documentation in the `docs/` directory or review the comprehensive Postman test collection for API usage examples.
