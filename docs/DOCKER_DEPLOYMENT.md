# Docker Deployment Guide

This guide explains how to build and deploy the Weapon API using Docker and DockerHub.

## Prerequisites

- Docker installed and running
- DockerHub account (free at [hub.docker.com](https://hub.docker.com))
- Git repository access

## Quick Start

### 1. Login to DockerHub
```bash
docker login
```

### 2. Build the Image
```bash
# Replace 'yourusername' with your DockerHub username
./docker-build.sh yourusername v1.0.0
```

### 3. Test Locally (Optional)
```bash
docker run -p 8080:8080 yourusername/weapon-api:v1.0.0
```

### 4. Push to DockerHub
```bash
./docker-push.sh yourusername v1.0.0
```

## Manual Docker Commands

If you prefer to use Docker commands directly:

### Build
```bash
# Build and tag the image
docker build -t yourusername/weapon-api:latest .
docker build -t yourusername/weapon-api:v1.0.0 .
```

### Test
```bash
# Test the image locally
docker run -p 8080:8080 yourusername/weapon-api:latest

# Test with environment variables
docker run -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Database=weaponapi;Username=weaponapi_user;Password=Dev123!@#" \
  yourusername/weapon-api:latest
```

### Push
```bash
# Push to DockerHub
docker push yourusername/weapon-api:latest
docker push yourusername/weapon-api:v1.0.0
```

## Using the Published Image

Once published, others can use your image:

### Docker Run
```bash
docker pull yourusername/weapon-api:latest
docker run -p 8080:8080 yourusername/weapon-api:latest
```

### Docker Compose
```yaml
version: '3.8'
services:
  weapon-api:
    image: yourusername/weapon-api:latest
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Host=postgres;Database=weaponapi;Username=weaponapi_user;Password=Dev123!@#
    depends_on:
      - postgres

  postgres:
    image: postgres:15
    environment:
      POSTGRES_DB: weaponapi
      POSTGRES_USER: weaponapi_user
      POSTGRES_PASSWORD: Dev123!@#
    ports:
      - "5432:5432"
```

## Image Information

- **Base Image**: mcr.microsoft.com/dotnet/aspnet:9.0
- **Platform**: linux/amd64
- **Exposed Port**: 8080
- **Entry Point**: dotnet WeaponApi.Api.dll
- **User**: Non-root user (appuser) for security

## Environment Variables

The container accepts these environment variables:

- `ASPNETCORE_ENVIRONMENT` - Development/Production
- `ASPNETCORE_URLS` - Binding URLs (default: http://+:8080)
- `ConnectionStrings__DefaultConnection` - Database connection string
- `JwtSettings__SecretKey` - JWT signing key
- `JwtSettings__Issuer` - JWT issuer
- `JwtSettings__Audience` - JWT audience
- `JwtSettings__ExpiryInMinutes` - Token expiry time

## Security Considerations

1. **Non-root User**: Image runs as non-root user `appuser`
2. **Secrets**: Never include sensitive data in the image
3. **Environment Variables**: Use environment variables for configuration
4. **Network**: Use Docker networks for service communication
5. **Volumes**: Use volumes for persistent data

## Troubleshooting

### Common Issues

1. **Permission Denied**
   ```bash
   chmod +x docker-build.sh docker-push.sh
   ```

2. **Docker Login Required**
   ```bash
   docker login
   ```

3. **Image Not Found**
   - Ensure you've built the image first
   - Check image name and tag

4. **Port Already in Use**
   ```bash
   # Use a different port
   docker run -p 8081:8080 yourusername/weapon-api:latest
   ```

### Logs
```bash
# View container logs
docker logs container-name

# Follow logs in real-time
docker logs -f container-name
```

## Production Considerations

1. **Use specific version tags** instead of `latest` in production
2. **Set up health checks** for container orchestration
3. **Use secrets management** for sensitive configuration
4. **Implement proper logging** and monitoring
5. **Consider using multi-stage builds** for smaller images (already implemented)

## Additional Resources

- [Docker Official Documentation](https://docs.docker.com/)
- [DockerHub Documentation](https://docs.docker.com/docker-hub/)
- [.NET Docker Images](https://hub.docker.com/_/microsoft-dotnet)
