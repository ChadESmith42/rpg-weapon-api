#!/bin/bash

# Weapon API Docker Push Script
# Usage: ./docker-push.sh [username] [version]
#
# Examples:
#   ./docker-push.sh myusername latest
#   ./docker-push.sh myusername v1.0.0

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
    echo "Usage: ./docker-push.sh [username] [version]"
    exit 1
fi

FULL_IMAGE_NAME="$DOCKERHUB_USERNAME/$IMAGE_NAME:$VERSION"

# Check if image exists locally
if ! docker images | grep -q "$DOCKERHUB_USERNAME/$IMAGE_NAME"; then
    print_error "Image not found locally. Build it first with:"
    echo "./docker-build.sh $DOCKERHUB_USERNAME $VERSION"
    exit 1
fi

print_status "Checking Docker login status..."
if ! docker info | grep -q "Username:"; then
    print_warning "Not logged in to Docker. Please login first:"
    echo "docker login"
    exit 1
fi

print_status "Pushing Docker image: $FULL_IMAGE_NAME"

# Push the specific version
docker push "$FULL_IMAGE_NAME"

if [ $? -eq 0 ]; then
    print_success "Successfully pushed $FULL_IMAGE_NAME"
else
    print_error "Failed to push $FULL_IMAGE_NAME"
    exit 1
fi

# Push latest tag if version is not latest
if [ "$VERSION" != "latest" ]; then
    LATEST_IMAGE_NAME="$DOCKERHUB_USERNAME/$IMAGE_NAME:latest"
    if docker images | grep -q "$LATEST_IMAGE_NAME"; then
        print_status "Pushing latest tag: $LATEST_IMAGE_NAME"
        docker push "$LATEST_IMAGE_NAME"

        if [ $? -eq 0 ]; then
            print_success "Successfully pushed $LATEST_IMAGE_NAME"
        else
            print_error "Failed to push $LATEST_IMAGE_NAME"
        fi
    fi
fi

echo
print_success "Push completed successfully!"
print_status "Your image is now available at:"
echo "https://hub.docker.com/r/$DOCKERHUB_USERNAME/$IMAGE_NAME"

echo
print_status "Others can now pull your image with:"
echo "docker pull $FULL_IMAGE_NAME"
