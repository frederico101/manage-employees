#!/bin/bash

echo "=========================================="
echo "Employee Management System - Docker Setup"
echo "=========================================="
echo ""

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker daemon is not running. Please start Docker Desktop first."
    exit 1
fi

echo "✅ Docker is running"
echo ""

# Stop and remove existing containers
echo "Cleaning up existing containers..."
docker-compose down 2>/dev/null

# Build and start containers
echo "Building and starting containers..."
docker-compose up --build -d

echo ""
echo "⏳ Waiting for services to start..."
sleep 10

# Check container status
echo ""
echo "Container Status:"
docker-compose ps

echo ""
echo "=========================================="
echo "Services are starting up!"
echo "=========================================="
echo ""
echo "📝 Next steps:"
echo ""
echo "1. Wait for all containers to be healthy (about 30-60 seconds)"
echo "2. Run database migrations manually:"
echo "   docker-compose exec api dotnet ef database update --project /src/EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj --startup-project /src/EmployeeManagement.API/EmployeeManagement.API.csproj"
echo ""
echo "3. Access the application:"
echo "   - Frontend: http://localhost:3000"
echo "   - API: http://localhost:8080"
echo "   - Swagger: http://localhost:8080"
echo ""
echo "4. View logs:"
echo "   docker-compose logs -f"
echo ""
echo "5. Stop services:"
echo "   docker-compose down"
echo ""

