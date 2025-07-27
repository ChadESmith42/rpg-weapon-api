# PostgreSQL Docker Setup for Weapon API

This directory contains Docker configuration for running PostgreSQL database for the Weapon API project.

## Quick Start

### 1. Start PostgreSQL Only

```bash
# Start just the PostgreSQL database
docker-compose up postgres -d

# View logs
docker-compose logs -f postgres
```

### 2. Start PostgreSQL with pgAdmin

```bash
# Start PostgreSQL and pgAdmin for database management
docker-compose up postgres pgadmin -d
```

### 3. Access Services

- **PostgreSQL Database**: `localhost:5432`
- **pgAdmin Web Interface**: `http://localhost:8080`
  - Email: `admin@weaponapi.com`
  - Password: `admin123`

## Database Connection Details

| Property | Value |
|----------|-------|
| Host | `localhost` (or `postgres` from within Docker) |
| Port | `5432` |
| Database | `weaponapi` |
| Username | `weaponapi_user` |
| Password | `Dev123!@#` |

## Connection Strings

### For local development (.NET app running outside Docker)

```text
Host=localhost;Database=weaponapi;Username=weaponapi_user;Password=Dev123!@#
```

### For containerized app (when .NET app runs in Docker)

```text
Host=postgres;Database=weaponapi;Username=weaponapi_user;Password=Dev123!@#
```

## Useful Commands

```bash
# Start services
docker-compose up -d

# Stop services
docker-compose down

# View logs
docker-compose logs -f postgres

# Access PostgreSQL shell
docker-compose exec postgres psql -U weaponapi_user -d weaponapi

# Rebuild containers
docker-compose build --no-cache

# Clean up (removes volumes - data will be lost!)
docker-compose down -v
```

## Directory Structure

```text
docker/
├── postgres/
│   └── init/
│       └── 01-init-weaponapi.sql     # Database initialization script
├── Dockerfile.postgres               # PostgreSQL Dockerfile
├── docker-compose.yml               # Docker Compose configuration
└── .env.template                    # Environment variables template
```

## Database Initialization

The PostgreSQL container will automatically run scripts in `docker/postgres/init/` when first created:

- `01-init-weaponapi.sql`: Creates database, extensions, and sets permissions

## Environment Variables

Copy `.env.template` to `.env` and modify as needed:

```bash
cp .env.template .env
```

## Health Checks

The PostgreSQL service includes health checks that verify the database is ready before starting dependent services.

## Data Persistence

Database data is persisted in Docker volumes:

- `postgres_data`: PostgreSQL data
- `pgadmin_data`: pgAdmin configuration

## Troubleshooting

### PostgreSQL won't start

1. Check if port 5432 is already in use: `lsof -i :5432`
2. View logs: `docker-compose logs postgres`
3. Ensure Docker has enough resources allocated

### Can't connect to database

1. Verify container is running: `docker-compose ps`
2. Check health status: `docker-compose ps postgres`
3. Test connection: `docker-compose exec postgres pg_isready -U weaponapi_user`

### Reset database

```bash
docker-compose down -v  # Warning: This deletes all data!
docker-compose up postgres -d
```
