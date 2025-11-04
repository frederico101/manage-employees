# API Documentation

## Base URL
```
http://localhost:8080/api
```

## Authentication

All endpoints except `/auth/register` and `/auth/login` require JWT authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

## Endpoints

### Authentication

#### Register New Employee
```http
POST /api/auth/register
Content-Type: application/json
```

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "docNumber": "123456789",
  "password": "Password123!",
  "dateOfBirth": "1990-01-01",
  "phones": [
    {
      "number": "1234567890",
      "type": 0
    }
  ],
  "role": 0
}
```

**Password Requirements:**
- Minimum 8 characters
- At least one uppercase letter (A-Z)
- At least one lowercase letter (a-z)
- At least one digit (0-9)
- At least one special character (!@#$%^&*(),.?":{}|<>)

**Response:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "docNumber": "123456789",
    "phones": [
      {
        "number": "1234567890",
        "type": 0
      }
    ],
    "managerId": null,
    "managerName": null,
    "role": 0,
    "dateOfBirth": "1990-01-01T00:00:00",
    "age": 35,
    "createdAt": "2025-11-04T00:00:00Z",
    "updatedAt": "2025-11-04T00:00:00Z"
  },
  "expiresAt": "2025-11-05T00:00:00Z"
}
```

**Error Responses:**
- `400 Bad Request` - Validation errors
- `500 Internal Server Error` - Server error

#### Login
```http
POST /api/auth/login
Content-Type: application/json
```

**Request Body:**
```json
{
  "email": "john.doe@example.com",
  "password": "Password123!"
}
```

**Response:** `200 OK`
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    ...
  },
  "expiresAt": "2025-11-05T00:00:00Z"
}
```

**Error Responses:**
- `401 Unauthorized` - Invalid credentials

### Employees

#### List Employees
```http
GET /api/employees?managerId={id}&role={role}&searchTerm={term}&pageNumber={page}&pageSize={size}
Authorization: Bearer <token>
```

**Query Parameters:**
- `managerId` (optional): Filter by manager ID
- `role` (optional): Filter by role (0=Employee, 1=Leader, 2=Director)
- `searchTerm` (optional): Search in name or email
- `pageNumber` (optional, default: 1): Page number
- `pageSize` (optional, default: 10): Items per page

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    ...
  }
]
```

#### Get Employee by ID
```http
GET /api/employees/{id}
Authorization: Bearer <token>
```

**Response:** `200 OK`
```json
{
  "id": 1,
  "firstName": "John",
  "lastName": "Doe",
  ...
}
```

**Error Responses:**
- `404 Not Found` - Employee not found

#### Create Employee
```http
POST /api/employees
Authorization: Bearer <token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "docNumber": "987654321",
  "password": "Password123!",
  "dateOfBirth": "1992-05-15",
  "phones": [
    {
      "number": "9876543210",
      "type": 0
    }
  ],
  "role": 0,
  "managerId": 1
}
```

**Permission Rules:**
- Users cannot create employees with higher or equal roles
- Employee cannot create Leader or Director
- Leader cannot create Director

**Response:** `201 Created`
```json
{
  "id": 2,
  "firstName": "Jane",
  ...
}
```

**Error Responses:**
- `400 Bad Request` - Validation errors
- `401 Unauthorized` - Insufficient permissions
- `409 Conflict` - Email or document number already exists

#### Update Employee
```http
PUT /api/employees/{id}
Authorization: Bearer <token>
Content-Type: application/json
```

**Request Body:**
```json
{
  "firstName": "Jane",
  "lastName": "Smith",
  "email": "jane.smith@example.com",
  "dateOfBirth": "1992-05-15",
  "phones": [
    {
      "number": "9876543210",
      "type": 0
    }
  ],
  "managerId": 1
}
```

**Permission Rules:**
- Users cannot update employees with higher or equal roles

**Response:** `200 OK`
```json
{
  "id": 2,
  "firstName": "Jane",
  ...
}
```

**Error Responses:**
- `400 Bad Request` - Validation errors
- `401 Unauthorized` - Insufficient permissions
- `404 Not Found` - Employee not found

#### Delete Employee
```http
DELETE /api/employees/{id}
Authorization: Bearer <token>
```

**Permission Rules:**
- Users cannot delete employees with higher or equal roles

**Response:** `204 No Content`

**Error Responses:**
- `401 Unauthorized` - Insufficient permissions
- `404 Not Found` - Employee not found

### Health Check

#### Health Check
```http
GET /health
```

**Response:** `200 OK`
```json
{
  "status": "Healthy",
  "timestamp": "2025-11-04T00:00:00Z"
}
```

## Data Models

### Employee Role
- `0` - Employee
- `1` - Leader
- `2` - Director

### Phone Type
- `0` - Mobile
- `1` - Home
- `2` - Work

## Error Responses

All error responses follow this format:

```json
{
  "error": {
    "message": "Error message",
    "details": "Additional error details (optional)",
    "statusCode": 400,
    "timestamp": "2025-11-04T00:00:00Z"
  }
}
```

## Rate Limiting

Currently no rate limiting is implemented. Consider adding rate limiting for production use.

## Swagger UI

Interactive API documentation is available at:
```
http://localhost:8080
```

You can test all endpoints directly from the Swagger UI. Click "Authorize" and enter your JWT token to test protected endpoints.

