#!/bin/bash

# Weapon API - Postman Testing Setup Script
# This script helps set up the environment for running Postman tests

set -e  # Exit on any error

echo "🚀 Weapon API - Postman Testing Setup"
echo "====================================="
echo

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is running
check_docker() {
    print_status "Checking Docker installation..."
    if ! command -v docker &> /dev/null; then
        print_error "Docker is not installed. Please install Docker first."
        exit 1
    fi

    if ! docker info &> /dev/null; then
        print_error "Docker is not running. Please start Docker first."
        exit 1
    fi

    print_success "Docker is running"
}

# Check if .NET is installed
check_dotnet() {
    print_status "Checking .NET installation..."
    if ! command -v dotnet &> /dev/null; then
        print_error ".NET is not installed. Please install .NET 9.0 or later."
        exit 1
    fi

    DOTNET_VERSION=$(dotnet --version)
    print_success ".NET version $DOTNET_VERSION is installed"
}

# Start PostgreSQL container
start_postgres() {
    print_status "Starting PostgreSQL database..."

    if [ -f "docker-compose.yml" ]; then
        docker-compose up -d postgres
        print_success "PostgreSQL container started"
    else
        print_error "docker-compose.yml not found. Please run this script from the project root."
        exit 1
    fi

    # Wait for PostgreSQL to be ready
    print_status "Waiting for PostgreSQL to be ready..."
    sleep 5

    # Check if PostgreSQL is responding
    if docker-compose exec -T postgres pg_isready -h localhost -U postgres &> /dev/null; then
        print_success "PostgreSQL is ready"
    else
        print_warning "PostgreSQL may still be starting up. Please wait a moment and try again."
    fi
}

# Build the application
build_application() {
    print_status "Building Weapon API application..."

    if [ -f "WeaponApi.sln" ]; then
        dotnet restore WeaponApi.sln
        dotnet build WeaponApi.sln --configuration Release
        print_success "Application built successfully"
    else
        print_error "WeaponApi.sln not found. Please run this script from the project root."
        exit 1
    fi
}

# Check if API is running
check_api_running() {
    print_status "Checking if Weapon API is running..."

    if curl -s http://localhost:5297/health &> /dev/null; then
        print_success "Weapon API is already running at http://localhost:5297"
        return 0
    elif curl -k -s https://localhost:7020/health &> /dev/null; then
        print_success "Weapon API is already running at https://localhost:7020"
        return 0
    else
        return 1
    fi
}

# Start the API in background
start_api() {
    if ! check_api_running; then
        print_status "Starting Weapon API..."

        # Start the API in the background
        nohup dotnet run --project WeaponApi.Api --configuration Release > api.log 2>&1 &
        API_PID=$!

        print_status "Waiting for API to start..."
        sleep 10

        # Check if API is now running
        if check_api_running; then
            print_success "Weapon API started successfully (PID: $API_PID)"
            echo $API_PID > api.pid
        else
            print_error "Failed to start Weapon API. Check api.log for details."
            exit 1
        fi
    fi
}

# Display Postman import instructions
show_postman_instructions() {
    echo
    echo "📋 Postman Import Instructions"
    echo "=============================="
    echo
    echo "1. Open Postman application"
    echo
    echo "2. Import Collection:"
    echo "   - Click 'Import' button"
    echo "   - Select 'Upload Files' tab"
    echo "   - Choose file: $(pwd)/postman/WeaponAPI-Comprehensive-Testing.postman_collection.json"
    echo
    echo "3. Import Environment:"
    echo "   - Click 'Import' button again"
    echo "   - Select 'Upload Files' tab"
    echo "   - Choose file: $(pwd)/postman/WeaponAPI-Testing.postman_environment.json"
    echo
    echo "4. Select Environment:"
    echo "   - In top-right dropdown, select 'Weapon API Testing'"
    echo
    echo "5. Run Tests:"
    echo "   - Execute folders in sequence for best results"
    echo "   - Or use Collection Runner for automated testing"
    echo
}

# Display API endpoints
show_api_info() {
    echo
    echo "🔗 API Information"
    echo "=================="
    echo
    echo "API Base URL: http://localhost:5297"
    echo "Health Check: http://localhost:5297/health"
    echo "Swagger UI: http://localhost:5297/swagger"
    echo
    echo "Available Endpoints:"
    echo "  Authentication:"
    echo "    POST /api/auth/register"
    echo "    POST /api/auth/login"
    echo "    GET  /api/auth/profile"
    echo "    POST /api/auth/refresh"
    echo "    POST /api/auth/logout"
    echo
    echo "  Weapons:"
    echo "    GET    /api/weapons"
    echo "    POST   /api/weapons"
    echo "    GET    /api/weapons/{id}"
    echo "    DELETE /api/weapons/{id}"
    echo "    GET    /api/weapons/create-random"
    echo "    POST   /api/weapons/{id}/damage"
    echo "    POST   /api/weapons/{id}/repair"
    echo "    POST   /api/weapons/estimate-repair"
    echo
}

# Display cleanup instructions
show_cleanup_instructions() {
    echo
    echo "🧹 Cleanup Instructions"
    echo "======================="
    echo
    echo "When finished testing:"
    echo
    echo "1. Stop the API:"
    if [ -f "api.pid" ]; then
        API_PID=$(cat api.pid)
        echo "   kill $API_PID"
    else
        echo "   Use Ctrl+C if running in foreground, or find and kill the dotnet process"
    fi
    echo
    echo "2. Stop PostgreSQL:"
    echo "   docker-compose down"
    echo
    echo "3. Clean up test data:"
    echo "   Run the 'Cleanup Operations' folder in Postman"
    echo
}

# Main execution
main() {
    echo "This script will:"
    echo "1. Check prerequisites (Docker, .NET)"
    echo "2. Start PostgreSQL database"
    echo "3. Build and start the Weapon API"
    echo "4. Provide Postman import instructions"
    echo

    read -p "Continue? (y/N) " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        echo "Setup cancelled."
        exit 0
    fi

    echo
    check_docker
    check_dotnet
    start_postgres
    build_application
    start_api

    print_success "Setup completed successfully!"

    show_api_info
    show_postman_instructions
    show_cleanup_instructions

    echo
    echo "🎉 Your Weapon API testing environment is ready!"
    echo
    echo "API Log: $(pwd)/api.log"
    echo "API PID: $(cat api.pid 2>/dev/null || echo 'Not available')"
    echo
}

# Run main function
main "$@"
