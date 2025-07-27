# .NET 8 Weapon API Setup Instructions

Confidence: 97%

Perfect! Now I have all the details I need. Here are the complete step-by-step instructions to create your .NET 8 Web API with PostgreSQL, JWT authentication, and Docker:

## Step-by-Step Instructions for .NET 8 Weapon API

### Phase 1: Project Setup and Structure

#### Step 1: Create Solution and Projects

```bash
# Navigate to your project directory
cd /Users/chadsmith/repos/weapon-api

# Create the solution
dotnet new sln -n WeaponApi

# Create the four Clean Architecture projects
dotnet new classlib -n WeaponApi.Domain -f net8.0
dotnet new classlib -n WeaponApi.Application -f net8.0
dotnet new classlib -n WeaponApi.Infrastructure -f net8.0
dotnet new webapi -n WeaponApi.Api -f net8.0

# Create test projects
mkdir -p tests
dotnet new classlib -n WeaponApi.UnitTests -f net8.0 -o tests/WeaponApi.UnitTests
dotnet new classlib -n WeaponApi.IntegrationTests -f net8.0 -o tests/WeaponApi.IntegrationTests

# Add projects to solution
dotnet sln add WeaponApi.Domain/WeaponApi.Domain.csproj
dotnet sln add WeaponApi.Application/WeaponApi.Application.csproj
dotnet sln add WeaponApi.Infrastructure/WeaponApi.Infrastructure.csproj
dotnet sln add WeaponApi.Api/WeaponApi.Api.csproj
dotnet sln add tests/WeaponApi.UnitTests/WeaponApi.UnitTests.csproj
dotnet sln add tests/WeaponApi.IntegrationTests/WeaponApi.IntegrationTests.csproj
```

#### Step 2: Set Up Project Dependencies**

```bash
# Application depends on Domain
cd WeaponApi.Application
dotnet add reference ../WeaponApi.Domain/WeaponApi.Domain.csproj

# Infrastructure depends on Application and Domain
cd ../WeaponApi.Infrastructure
dotnet add reference ../WeaponApi.Application/WeaponApi.Application.csproj
dotnet add reference ../WeaponApi.Domain/WeaponApi.Domain.csproj

# API depends on Infrastructure
cd ../WeaponApi.Api
dotnet add reference ../WeaponApi.Infrastructure/WeaponApi.Infrastructure.csproj

# Test projects dependencies
cd ../tests/WeaponApi.UnitTests
dotnet add reference ../../WeaponApi.Domain/WeaponApi.Domain.csproj
dotnet add reference ../../WeaponApi.Application/WeaponApi.Application.csproj

cd ../WeaponApi.IntegrationTests
dotnet add reference ../../WeaponApi.Api/WeaponApi.Api.csproj
```

### Phase 2: Install Required NuGet Packages

#### Step 3: Add NuGet Packages**

```bash
# Domain project (minimal dependencies)
cd ../../WeaponApi.Domain
# No external dependencies needed for Domain

# Application project
cd ../WeaponApi.Application
dotnet add package MediatR
dotnet add package FluentValidation
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions

# Infrastructure project
cd ../WeaponApi.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package BCrypt.Net-Next
dotnet add package System.IdentityModel.Tokens.Jwt
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# API project
cd ../WeaponApi.Api
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore

# Test projects
cd ../tests/WeaponApi.UnitTests
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package FluentAssertions
dotnet add package Moq

cd ../WeaponApi.IntegrationTests
dotnet add package Microsoft.NET.Test.Sdk
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package FluentAssertions
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package Testcontainers.PostgreSql
dotnet add package NetArchTest.Rules
```

### Phase 3: Domain Layer Implementation

#### Step 4: Create Domain Entities and Value Objects**

Create the following files in `WeaponApi.Domain`:

1. **Create `DomainReference.cs`** - Marker file for architecture tests
2. **Create `Weapon/WeaponId.cs`** - Strongly-typed ID as value object
3. **Create `Weapon/Weapon.cs`** - Weapon aggregate root with factory method
4. **Create `User/UserId.cs`** - Strongly-typed ID for User
5. **Create `User/User.cs`** - User aggregate root with authentication methods
6. **Create `User/Email.cs`** - Email value object with validation
7. **Create `Common/IDomainEvent.cs`** - Interface for domain events
8. **Create `Weapon/WeaponCreatedEvent.cs`** - Domain event for weapon creation

### Phase 4: Application Layer Implementation

#### Step 5: Create Application Services and Interfaces**

Create the following in `WeaponApi.Application`:

1. **Create `ApplicationReference.cs`** - Marker file
2. **Create `Common/IRepository.cs`** - Generic repository interface
3. **Create `Weapon/IWeaponRepository.cs`** - Weapon-specific repository interface
4. **Create `User/IUserRepository.cs`** - User-specific repository interface
5. **Create `Authentication/IJwtTokenService.cs`** - JWT token service interface
6. **Create `Authentication/IPasswordHashService.cs`** - Password hashing interface
7. **Create `Weapon/Commands/CreateWeaponCommand.cs`** - MediatR command
8. **Create `Weapon/Commands/CreateWeaponCommandHandler.cs`** - Command handler
9. **Create `Weapon/Queries/GetWeaponQuery.cs`** - MediatR query
10. **Create `Weapon/Queries/GetWeaponQueryHandler.cs`** - Query handler
11. **Create `User/Commands/RegisterUserCommand.cs`** - User registration command
12. **Create `User/Commands/LoginUserCommand.cs`** - User login command
13. **Create `User/DTOs/WeaponDto.cs`** - Data transfer objects
14. **Create `User/DTOs/UserDto.cs`** - User DTOs
15. **Create `Common/Validators/CreateWeaponCommandValidator.cs`** - FluentValidation validators

### Phase 5: Infrastructure Layer Implementation

#### Step 6: Create Database Context and Repositories**

Create the following in `WeaponApi.Infrastructure`:

1. **Create `InfrastructureReference.cs`** - Marker file
2. **Create `Persistence/WeaponApiDbContext.cs`** - EF Core DbContext
3. **Create `Persistence/Configurations/WeaponConfiguration.cs`** - EF entity configuration
4. **Create `Persistence/Configurations/UserConfiguration.cs`** - User entity configuration
5. **Create `Repositories/WeaponRepository.cs`** - Weapon repository implementation
6. **Create `Repositories/UserRepository.cs`** - User repository implementation
7. **Create `Authentication/JwtTokenService.cs`** - JWT token service implementation
8. **Create `Authentication/PasswordHashService.cs`** - BCrypt password hashing
9. **Create `DependencyInjection.cs`** - Service registration for Infrastructure

### Phase 7: API Layer Implementation

#### Step 7: Create Controllers and API Configuration**

Create/modify the following in `WeaponApi.Api`:

1. **Create `ApiReference.cs`** - Marker file
2. **Create `Controllers/WeaponsController.cs`** - Weapon CRUD endpoints
3. **Create `Controllers/AuthController.cs`** - Authentication endpoints (register, login, profile)
4. **Create `Models/Requests/CreateWeaponRequest.cs`** - API request models
5. **Create `Models/Requests/RegisterUserRequest.cs`** - Registration request
6. **Create `Models/Requests/LoginUserRequest.cs`** - Login request
7. **Create `Models/Responses/WeaponResponse.cs`** - API response models
8. **Create `Models/Responses/AuthResponse.cs`** - Authentication response
9. **Create `Middleware/ExceptionHandlingMiddleware.cs`** - Global exception handling
10. **Modify `Program.cs`** - Configure services, JWT, database, middleware
11. **Create `appsettings.json`** - Configuration for JWT, database connection
12. **Create `appsettings.Development.json`** - Development-specific settings

### Phase 8: Database Migrations

#### Step 8: Create and Apply EF Core Migrations**

```bash
# Navigate to API project (where DbContext is registered)
cd ../../WeaponApi.Api

# Create initial migration
dotnet ef migrations add InitialCreate --project ../WeaponApi.Infrastructure --startup-project .

# The migration files will be created in WeaponApi.Infrastructure/Migrations/
```

### Phase 9: Docker Configuration

#### Step 9: Create Docker Files**

Create the following files in the root directory:

1. **Create `Dockerfile`** - Multi-stage build for the .NET API
2. **Create `docker-compose.yml`** - Container orchestration for API and PostgreSQL
3. **Create `docker-compose.override.yml`** - Development overrides
4. **Create `.dockerignore`** - Exclude unnecessary files from Docker context

### Phase 10: Testing Setup

#### Step 10: Create Test Files**

Create the following test files:

1. **In `tests/WeaponApi.UnitTests`:**
   - `Domain/WeaponTests.cs` - Unit tests for Weapon aggregate
   - `Domain/UserTests.cs` - Unit tests for User aggregate
   - `Application/CreateWeaponCommandHandlerTests.cs` - Command handler tests

2. **In `tests/WeaponApi.IntegrationTests`:**
   - `ArchitectureTests.cs` - NetArchTest rules for Clean Architecture
   - `WeaponsControllerTests.cs` - Integration tests for weapon endpoints
   - `AuthControllerTests.cs` - Integration tests for auth endpoints

### Phase 11: Configuration and Startup

#### Step 11: Configure the Application**

```bash
# Create development certificate (if needed)
dotnet dev-certs https --trust

# Build the solution
cd /Users/chadsmith/repos/weapon-api
dotnet build

# Run migrations (after creating DbContext and models)
cd WeaponApi.Api
dotnet ef database update --project ../WeaponApi.Infrastructure
```

### Phase 12: Docker Development Environment

#### Step 12: Set Up Docker Environment**

```bash
# Build and run with Docker Compose
docker-compose up --build

# To run in detached mode
docker-compose up -d

# To stop containers
docker-compose down

# To view logs
docker-compose logs -f weapon-api
```

### Phase 13: Testing and Validation

#### Step 13: Run Tests and Validate Architecture**

```bash
# Run unit tests
dotnet test tests/WeaponApi.UnitTests/

# Run integration tests (requires Docker containers to be running)
dotnet test tests/WeaponApi.IntegrationTests/

# Run all tests
dotnet test
```

### Key Implementation Notes

1. **Domain Models**: Implement `Weapon` and `User` as aggregate roots with private constructors and static factory methods
2. **Strongly-Typed IDs**: Use `WeaponId` and `UserId` value objects instead of primitive GUIDs
3. **Repository Pattern**: Implement repositories only for aggregate roots, with business-focused method names
4. **JWT Configuration**: Include proper JWT configuration with secret key, issuer, and audience
5. **Database Connection**: Use connection string for PostgreSQL with proper error handling
6. **API Endpoints**: Implement RESTful endpoints for weapons and authentication
7. **Error Handling**: Global exception middleware for consistent error responses
8. **Validation**: Use FluentValidation for command validation in the Application layer
9. **Docker Networking**: Configure proper networking between API and database containers
10. **Environment Variables**: Use environment variables for sensitive configuration

This structure follows Clean Architecture principles, DDD patterns, and the coding standards specified in your instruction files. Each phase builds upon the previous one, ensuring a maintainable and testable application.
