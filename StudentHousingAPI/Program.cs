using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudentHousingAPI.Data;
using StudentHousingAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// Configure CORS for MAUI app
// WARNING: For production, restrict to specific origins instead of AllowAnyOrigin()
// Example: policy.WithOrigins("https://yourdomain.com").AllowAnyMethod().AllowAnyHeader();
builder.Services.AddCors(options =>
{
    options.AddPolicy("MauiApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Register services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Student Housing API", 
        Version = "v1",
        Description = "API for Student Housing Management System"
    });

    // Configure JWT authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var passwordService = services.GetRequiredService<IPasswordService>();
    
    // Ensure database is created and apply migrations
    context.Database.Migrate();
    
    // Seed data
    await SeedData(context, passwordService);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Student Housing API V1");
    });
}

app.UseHttpsRedirection();

app.UseCors("MauiApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task SeedData(ApplicationDbContext context, IPasswordService passwordService)
{
    // Check if data already exists
    if (context.Buildings.Any())
    {
        return; // Database has been seeded
    }

    // Seed Building
    var building = new StudentHousingAPI.Models.Building
    {
        Name = "Sunrise Residence",
        Address = "123 University Ave, College Town, ST 12345"
    };
    context.Buildings.Add(building);
    await context.SaveChangesAsync();

    // Seed Users
    var studentUser = new StudentHousingAPI.Models.User
    {
        Username = "john.student",
        Email = "john@example.com",
        PasswordHash = passwordService.HashPassword("password123"),
        Role = "Student",
        BuildingId = building.Id
    };

    var staffUser = new StudentHousingAPI.Models.User
    {
        Username = "jane.staff",
        Email = "jane@example.com",
        PasswordHash = passwordService.HashPassword("password123"),
        Role = "Staff",
        BuildingId = building.Id
    };

    var maintenanceUser = new StudentHousingAPI.Models.User
    {
        Username = "mike.maintenance",
        Email = "mike@example.com",
        PasswordHash = passwordService.HashPassword("password123"),
        Role = "Maintenance",
        BuildingId = building.Id
    };

    context.Users.AddRange(studentUser, staffUser, maintenanceUser);
    await context.SaveChangesAsync();

    // Seed a sample issue
    var issue = new StudentHousingAPI.Models.Issue
    {
        Description = "Water leak in the common bathroom on floor 2",
        SharedSpace = "Common Bathroom - Floor 2",
        Status = "Open",
        CreatedByUserId = studentUser.Id,
        BuildingId = building.Id,
        CreatedAt = DateTime.UtcNow
    };
    context.Issues.Add(issue);

    // Seed sample tasks
    var cleaningTask = new StudentHousingAPI.Models.CleaningTask
    {
        Description = "Clean the common kitchen area",
        SharedSpace = "Common Kitchen",
        Status = "Pending",
        BuildingId = building.Id,
        AssignedUserId = studentUser.Id,
        DueDate = DateTime.UtcNow.AddDays(2),
        CreatedAt = DateTime.UtcNow
    };

    var garbageTask = new StudentHousingAPI.Models.GarbageTask
    {
        Description = "Take out garbage bins to the street",
        Location = "Main entrance",
        Status = "Pending",
        BuildingId = building.Id,
        AssignedUserId = studentUser.Id,
        DueDate = DateTime.UtcNow.AddDays(1),
        CreatedAt = DateTime.UtcNow
    };

    context.CleaningTasks.Add(cleaningTask);
    context.GarbageTasks.Add(garbageTask);
    await context.SaveChangesAsync();
}
