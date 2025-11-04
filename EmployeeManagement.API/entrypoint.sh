#!/bin/bash
set -e

echo "Waiting for PostgreSQL to be ready..."
until pg_isready -h postgres -p 5432 -U postgres; do
    echo "PostgreSQL is unavailable - sleeping"
    sleep 2
done

echo "PostgreSQL is up!"

# Run migrations if source files are available
if [ -d "/src/EmployeeManagement.Infrastructure/Migrations" ]; then
    echo "Running database migrations..."
    cd /src
    dotnet ef database update --project EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj --startup-project EmployeeManagement.API/EmployeeManagement.API.csproj --context ApplicationDbContext || echo "Migrations may have already been applied"
    echo "Migrations completed."
else
    echo "⚠️  Migration files not found. Please run migrations manually:"
    echo "   docker-compose exec api dotnet ef database update --project /src/EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj --startup-project /src/EmployeeManagement.API/EmployeeManagement.API.csproj"
fi

echo "Starting application..."
cd /app

# Start the application
exec dotnet EmployeeManagement.API.dll

