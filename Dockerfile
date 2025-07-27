# Use the official ASP.NET Core runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["WeaponApi.Api/WeaponApi.Api.csproj", "WeaponApi.Api/"]
COPY ["WeaponApi.Application/WeaponApi.Application.csproj", "WeaponApi.Application/"]
COPY ["WeaponApi.Domain/WeaponApi.Domain.csproj", "WeaponApi.Domain/"]
COPY ["WeaponApi.Infrastructure/WeaponApi.Infrastructure.csproj", "WeaponApi.Infrastructure/"]

RUN dotnet restore "WeaponApi.Api/WeaponApi.Api.csproj"

# Copy all source code
COPY . .

# Publish the application directly (skip separate build step)
WORKDIR "/src/WeaponApi.Api"
RUN dotnet publish "WeaponApi.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Create the final runtime image
FROM base AS final
WORKDIR /app

# Create a non-root user for security
RUN adduser --disabled-password --home /app --gecos '' appuser && chown -R appuser /app
USER appuser

# Copy the published application
COPY --from=build /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "WeaponApi.Api.dll"]
