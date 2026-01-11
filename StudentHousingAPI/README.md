# Student Housing Management API

A comprehensive ASP.NET Core Web API for managing student housing facilities, including issue tracking, task management, and role-based access control.

## Features

### Authentication
- JWT-based authentication
- Role-based authorization (Student, Staff, Maintenance)
- Secure password hashing

### Issue Tracking
- Students can create and view their issues
- Staff can view, filter, and assign issues to maintenance
- Support for photo proof uploads (optional)
- Issue statuses: Open, InProgress, Resolved, Closed

### Task Management
- Cleaning and garbage tasks
- Students can view and complete assigned tasks
- Staff can assign tasks and verify completion
- Task statuses: Pending, Completed, Verified

### Dashboard (Staff Only)
- View issue counts by status
- Track overdue tasks
- Monitor tasks due today

## Technology Stack

- **.NET 8** - Framework
- **ASP.NET Core Web API** - API framework
- **Entity Framework Core** - ORM
- **SQLite** - Database (for development)
- **JWT Bearer** - Authentication
- **FluentValidation** - Request validation
- **Swagger/OpenAPI** - API documentation

## Getting Started

### Prerequisites

- .NET 8 SDK or higher
- A code editor (Visual Studio, VS Code, etc.)

### Installation

1. Clone the repository:
```bash
cd StudentHousingAPI
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the application:
```bash
dotnet run
```

The API will be available at `http://localhost:5000` (HTTP) or `https://localhost:5001` (HTTPS).

### Database Setup

The database is automatically created and seeded with sample data on first run. The seed data includes:

**Building:**
- Sunrise Residence (123 University Ave)

**Users:**
- Student: `john@example.com` / `password123`
- Staff: `jane@example.com` / `password123`
- Maintenance: `mike@example.com` / `password123`

**Sample Data:**
- 1 sample issue
- 1 cleaning task
- 1 garbage task

## API Documentation

### Swagger UI

Access the interactive API documentation at: `http://localhost:5000/swagger`

### Authentication

All endpoints (except `/api/auth/login`) require a JWT token.

**Login:**
```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "role": "Student",
  "userId": 1,
  "username": "john.student"
}
```

Use the token in subsequent requests:
```bash
Authorization: Bearer <token>
```

### Endpoints

#### Issues

- `GET /api/issues` - Get issues (filtered by role)
- `GET /api/issues/{id}` - Get specific issue
- `POST /api/issues` - Create issue (Student only)
- `PATCH /api/issues/{id}` - Update issue (Staff only)

#### Tasks

- `GET /api/tasks/cleaning` - Get cleaning tasks
- `GET /api/tasks/garbage` - Get garbage tasks
- `POST /api/tasks/cleaning` - Create cleaning task (Staff only)
- `POST /api/tasks/garbage` - Create garbage task (Staff only)
- `POST /api/tasks/cleaning/{id}/complete` - Complete cleaning task (Student only)
- `POST /api/tasks/garbage/{id}/complete` - Complete garbage task (Student only)
- `POST /api/tasks/cleaning/{id}/verify` - Verify cleaning task (Staff only)
- `POST /api/tasks/garbage/{id}/verify` - Verify garbage task (Staff only)

#### Dashboard

- `GET /api/dashboard` - Get dashboard statistics (Staff only)

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=studenthousing.db"
  },
  "JwtSettings": {
    "SecretKey": "YourSecretKeyHere",
    "Issuer": "StudentHousingAPI",
    "Audience": "StudentHousingClient",
    "ExpiryMinutes": "60"
  }
}
```

### CORS

CORS is enabled for all origins to support the .NET MAUI mobile app. Adjust the CORS policy in `Program.cs` for production use.

## Project Structure

```
StudentHousingAPI/
├── Controllers/        # API controllers
├── Data/              # Database context
├── DTOs/              # Data transfer objects
├── Models/            # Entity models
├── Services/          # Business logic services
├── Validators/        # FluentValidation validators
├── Migrations/        # EF Core migrations
├── Program.cs         # Application entry point
└── appsettings.json   # Configuration
```

## Development

### Creating Migrations

```bash
dotnet ef migrations add MigrationName
```

### Applying Migrations

Migrations are automatically applied on application startup, or manually:

```bash
dotnet ef database update
```

### Building

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

## Security Considerations

### For Production Deployment:

1. **JWT Secret Key**: Move `JwtSettings:SecretKey` to environment variables or a secure key management service (e.g., Azure Key Vault, AWS Secrets Manager)
   ```bash
   # Set via environment variable
   export JwtSettings__SecretKey="your-production-secret-key"
   ```

2. **Password Hashing**: The application uses BCrypt for secure password hashing with automatic salt generation

3. **HTTPS**: Always use HTTPS in production. The application is configured to redirect HTTP to HTTPS

4. **CORS**: Update the CORS policy in `Program.cs` to restrict origins to your specific domains:
   ```csharp
   policy.WithOrigins("https://yourdomain.com")
         .AllowAnyMethod()
         .AllowAnyHeader();
   ```

5. **Database**: Use a production-grade database (SQL Server, PostgreSQL) instead of SQLite

6. **Logging**: Ensure sensitive data (passwords, tokens) is never logged

7. **Rate Limiting**: Consider implementing rate limiting to prevent brute force attacks

## Production Configuration

### Environment Variables

Set these environment variables for production:

```bash
ConnectionStrings__DefaultConnection="Server=...;Database=...;User Id=...;Password=..."
JwtSettings__SecretKey="your-very-secure-secret-key-at-least-32-characters"
JwtSettings__Issuer="YourProductionIssuer"
JwtSettings__Audience="YourProductionAudience"
JwtSettings__ExpiryMinutes="60"
```

## License

This project is part of a Student Housing Management MVP system.
