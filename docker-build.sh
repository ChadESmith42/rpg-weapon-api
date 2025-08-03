#!/bin/bash

# Weapon API Docker Build and Push Script
# Usage: ./docker-build.sh [username] [version]
#
# Examples:
#   ./docker-build.sh myusername latest
#   ./docker-build.sh myusername v1.0.0

set -e

# Default values
DOCKERHUB_USERNAME=${1:-"your-username"}
VERSION=${2:-"latest"}
IMAGE_NAME="weapon-api"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

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

# Validate inputs
if [ "$DOCKERHUB_USERNAME" = "your-username" ]; then
    print_error "Please provide your DockerHub username"
    echo "Usage: ./docker-build.sh [username] [version]"
    exit 1
fi

FULL_IMAGE_NAME="$DOCKERHUB_USERNAME/$IMAGE_NAME:$VERSION"

print_status "Building Docker image: $FULL_IMAGE_NAME"

# Build the image
docker build -t "$FULL_IMAGE_NAME" .

if [ $? -eq 0 ]; then
    print_success "Docker image built successfully"
else
    print_error "Failed to build Docker image"
    exit 1
fi

# Also tag as latest if version is not latest
if [ "$VERSION" != "latest" ]; then
    print_status "Tagging as latest version"
    docker tag "$FULL_IMAGE_NAME" "$DOCKERHUB_USERNAME/$IMAGE_NAME:latest"
fi

print_status "Available images:"
docker images | grep "$DOCKERHUB_USERNAME/$IMAGE_NAME"

echo
print_status "To test the image locally:"
echo "docker run -p 8080:8080 $FULL_IMAGE_NAME"
echo

print_status "To push to DockerHub:"
echo "docker push $FULL_IMAGE_NAME"
if [ "$VERSION" != "latest" ]; then
    echo "docker push $DOCKERHUB_USERNAME/$IMAGE_NAME:latest"
fi

print_success "Build completed successfully!"
