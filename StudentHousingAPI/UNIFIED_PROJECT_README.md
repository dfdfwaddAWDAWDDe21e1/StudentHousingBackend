# Unified Student Housing Management System

This project combines the frontend web interface and backend API into a single unified application.

## Overview

The StudentHousingAPI now serves both:
1. **Backend API** - RESTful API endpoints for managing student housing
2. **Frontend Web Interface** - Modern, responsive web UI served from `wwwroot`

## Architecture

```
StudentHousingAPI/
├── Controllers/          # API controllers
├── wwwroot/             # Frontend static files
│   ├── index.html       # Main HTML page
│   ├── css/
│   │   └── styles.css   # Styling
│   └── js/
│       └── app.js       # Frontend JavaScript (API integration)
├── Program.cs           # Configured to serve static files
└── ...                  # Other backend files
```

## Features

### Frontend (wwwroot)
- **Responsive Web UI** with modern design
- **Authentication** - Login system with JWT tokens
- **Issues Management** - View and create issues (Students)
- **Tasks Management** - View and complete assigned tasks
- **Dashboard** - Statistics and metrics (Staff only)
- **Role-Based UI** - Different features based on user role

### Backend API
- RESTful API with JWT authentication
- Role-based authorization (Student, Staff, Maintenance)
- Issue tracking and task management
- SQLite database with EF Core
- Swagger documentation at `/swagger`

## Running the Application

### Prerequisites
- .NET 8 SDK or higher

### Start the Server

```bash
cd StudentHousingAPI
dotnet run
```

The application will be available at:
- **Web UI**: `http://localhost:5000` or `https://localhost:5001`
- **API**: `http://localhost:5000/api`
- **Swagger**: `http://localhost:5000/swagger` (Development mode only)

### Access the Web Interface

1. Open your browser and navigate to `http://localhost:5000`
2. You'll see the login page
3. Use one of the demo accounts:
   - **Student**: `john@example.com` / `password123`
   - **Staff**: `jane@example.com` / `password123`
   - **Maintenance**: `mike@example.com` / `password123`

## Configuration

### Static File Serving

The backend is configured in `Program.cs` to serve static files:

```csharp
// Serve static files from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// Fallback to index.html for SPA routing
app.MapFallbackToFile("index.html");
```

### API Base URL

The frontend automatically uses the same origin for API calls:

```javascript
const API_BASE_URL = window.location.origin + '/api';
```

This means the frontend will automatically work whether you're accessing:
- `http://localhost:5000`
- `https://localhost:5001`
- `https://yourdomain.com`

### CORS Configuration

CORS is already configured in `Program.cs` to allow the frontend to communicate with the API:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("MauiApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

**Note**: For production, restrict CORS to specific origins for security.

## Development

### Modifying the Frontend

1. Edit files in `wwwroot/`:
   - `index.html` - HTML structure
   - `css/styles.css` - Styling
   - `js/app.js` - JavaScript logic and API calls

2. Refresh your browser to see changes (no rebuild needed for static files)

### Modifying the Backend

1. Edit C# files (Controllers, Models, Services, etc.)
2. The application will hot-reload if using `dotnet watch run`
3. Or restart the application with `dotnet run`

### Building for Production

```bash
dotnet build -c Release
dotnet publish -c Release -o ./publish
```

The published output will include both the backend and frontend files.

## Security Considerations

### For Production Deployment

1. **CORS Policy**: Update the CORS policy in `Program.cs` to restrict origins:
   ```csharp
   policy.WithOrigins("https://yourdomain.com")
         .AllowAnyMethod()
         .AllowAnyHeader();
   ```

2. **HTTPS**: Always use HTTPS in production
3. **JWT Secret**: Use environment variables for the JWT secret key
4. **Database**: Use a production database (SQL Server, PostgreSQL) instead of SQLite
5. **Environment Variables**: Configure sensitive settings via environment variables

## API Endpoints

All API endpoints are accessible at `/api/*`:

- `POST /api/auth/login` - Login and get JWT token
- `GET /api/issues` - List issues (authenticated)
- `POST /api/issues` - Create issue (Student only)
- `GET /api/tasks/cleaning` - List cleaning tasks
- `GET /api/tasks/garbage` - List garbage tasks
- `POST /api/tasks/{type}/{id}/complete` - Complete task
- `GET /api/dashboard` - Get statistics (Staff only)

For complete API documentation, visit `/swagger` when running in development mode.

## Frontend Features by Role

### Student Users
- ✅ View their own issues
- ✅ Create new issues
- ✅ View assigned tasks
- ✅ Complete tasks

### Staff Users
- ✅ View all issues
- ✅ View all tasks
- ✅ Access dashboard with statistics
- ✅ (API supports assigning issues and verifying tasks)

### All Users
- ✅ Secure login/logout
- ✅ JWT token-based authentication
- ✅ Responsive design (mobile-friendly)

## Troubleshooting

### Frontend not loading
- Check that `wwwroot` folder exists with `index.html`
- Verify `UseDefaultFiles()` and `UseStaticFiles()` are called in `Program.cs`
- Check browser console for JavaScript errors

### API calls failing
- Verify the server is running
- Check browser console Network tab for failed requests
- Ensure JWT token is valid (check localStorage)
- Verify CORS is configured correctly

### Database errors
- Delete `studenthousing.db*` files and restart to recreate database
- Check migrations are applied: `dotnet ef database update`

## Migration from Separate Projects

This unified project replaces the previous setup where:
- **HousingApp** was a separate .NET MAUI desktop application
- **StudentHousingAPI** was a standalone backend API

Now, the web frontend is integrated directly into the API project, making deployment simpler and enabling seamless communication between frontend and backend.

## License

This project is part of a Student Housing Management MVP system.
