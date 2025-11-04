# Employee Management System

A full-stack employee management application built with .NET 8 and React.

## Features

- **Employee Management**: CRUD operations for employees
- **Role-Based Access Control**: Employee, Leader, Director roles with hierarchical permissions
- **Authentication**: JWT-based authentication
- **Multi-Phone Support**: Employees can have multiple phone numbers
- **Manager Relationships**: Employees can be assigned to managers
- **Age Validation**: Ensures employees are at least 18 years old
- **Document Uniqueness**: Email and document number must be unique

## Tech Stack

### Backend
- .NET 8
- PostgreSQL
- Entity Framework Core
- JWT Authentication
- Clean Architecture
- MediatR (CQRS)
- FluentValidation
- AutoMapper
- Serilog

### Frontend
- React 19 with TypeScript
- React Router
- Zustand (State Management)
- Axios
- CSS Modules

## Prerequisites

- Docker and Docker Compose
- .NET 8 SDK (for local development)
- Node.js 18+ and Yarn (for local development)

## Quick Start with Docker

1. **Clone the repository** (if needed)

2. **Start all services**:
   ```bash
   docker-compose up --build
   ```

3. **Access the application**:
   - Frontend: http://localhost:3000
   - API: http://localhost:8080
   - Swagger UI: http://localhost:8080
   - PostgreSQL: localhost:5432

4. **Run database migrations** (automatically runs on API startup)

## Manual Setup (Without Docker)

### Backend Setup

1. **Start PostgreSQL** (using Docker):
   ```bash
   docker-compose up postgres
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Run migrations**:
   ```bash
   dotnet ef database update --project EmployeeManagement.Infrastructure/EmployeeManagement.Infrastructure.csproj --startup-project EmployeeManagement.API/EmployeeManagement.API.csproj
   ```

4. **Run the API**:
   ```bash
   cd EmployeeManagement.API
   dotnet run
   ```

### Frontend Setup

1. **Install dependencies**:
   ```bash
   cd employee-management-frontend
   yarn install
   ```

2. **Start development server**:
   ```bash
   yarn start
   ```

## API Endpoints

### Authentication
- `POST /api/auth/register` - Register new employee
- `POST /api/auth/login` - Login and get JWT token

### Employees (Requires Authentication)
- `GET /api/employees` - List employees (with filters)
- `GET /api/employees/{id}` - Get employee by ID
- `POST /api/employees` - Create employee
- `PUT /api/employees/{id}` - Update employee
- `DELETE /api/employees/{id}` - Delete employee

### Health Check
- `GET /health` - Health check endpoint

## Role Hierarchy

- **Employee** (0): Basic employee role
- **Leader** (1): Can manage employees
- **Director** (2): Highest role, can manage all

**Permission Rules**:
- Users cannot create employees with higher or equal roles
- Users cannot update/delete employees with higher or equal roles

## Database Schema

The application uses PostgreSQL with the following main tables:
- `Employees` - Employee information
- `EmployeePhones` - Phone numbers (owned entity)

## Environment Variables

### API
- `ConnectionStrings__DefaultConnection` - PostgreSQL connection string
- `Jwt:SecretKey` - JWT signing key
- `Jwt:Issuer` - JWT issuer
- `Jwt:Audience` - JWT audience
- `Jwt:ExpirationMinutes` - Token expiration time

### Frontend
- `REACT_APP_API_URL` - Backend API URL (default: http://localhost:8080/api)

## Docker Services

- **postgres**: PostgreSQL database
- **api**: .NET 8 Web API
- **frontend**: React application served by Nginx

## Development

### Running Tests
```bash
# Backend tests (84 tests - 100% passing)
dotnet test EmployeeManagement.Tests/EmployeeManagement.Tests.csproj

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Frontend tests
cd employee-management-frontend
yarn test
```

### Test Coverage
- **Total Tests**: 84
- **Passing**: 84 (100%)
- **Coverage Areas**:
  - Domain entities and business rules
  - Application use cases (CQRS)
  - Request validators
  - Repository implementations
  - Service implementations (Password hashing, JWT)
  - API controllers
  - Authentication flows
  - Permission system

### Building for Production
```bash
# Backend
dotnet publish -c Release

# Frontend
cd employee-management-frontend
yarn build
```

## Project Structure

```
manage-employees/
├── EmployeeManagement.Domain/       # Domain entities and business logic
├── EmployeeManagement.Application/  # Use cases and DTOs
├── EmployeeManagement.Infrastructure/ # Data access and external services
├── EmployeeManagement.API/          # Web API controllers
├── employee-management-frontend/    # React application
└── docker-compose.yml               # Docker configuration
```

## Documentation

- [API Documentation](./API_DOCUMENTATION.md) - Complete API reference
- [Deployment Guide](./DEPLOYMENT.md) - Production deployment instructions

## Testing

The project includes comprehensive unit tests:

- **84 tests** covering all layers
- **100% passing rate**
- Domain, Application, Infrastructure, and API layers tested
- Use of xUnit, Moq, FluentAssertions, and FluentValidation.TestHelper

Run tests:
```bash
dotnet test EmployeeManagement.Tests/EmployeeManagement.Tests.csproj
```

## CI/CD (Optional)

CI/CD is not required by the project specifications, but can be added for automated testing and deployment if needed.

## Contributing

1. Create a feature branch
2. Make your changes
3. Add tests for new functionality
4. Ensure all tests pass
5. Submit a pull request

## License

This project is for demonstration purposes.

