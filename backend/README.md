# Learnyx Backend API

A robust ASP.NET Core 8 Web API for the Learnyx Learning Management System, built with modern architectural patterns and comprehensive features for course management, user authentication, real-time communication, and more.

## 🚀 Overview

Learnyx Backend is a comprehensive SaaS Learning Management System API that provides secure, scalable, and feature-rich endpoints for managing courses, users, assignments, real-time chat, and various integrations. Built with .NET 8, Entity Framework Core, and SignalR for real-time functionality.

## 📋 Table of Contents

- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Features](#features)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Architecture](#architecture)
- [API Endpoints](#api-endpoints)
- [Database Schema](#database-schema)
- [Authentication & Authorization](#authentication--authorization)
- [Real-time Features](#real-time-features)
- [External Integrations](#external-integrations)
- [Development](#development)
- [Testing](#testing)
- [Deployment](#deployment)

## 🛠 Technology Stack

- **Framework**: ASP.NET Core 8.0
- **Language**: C# 12.0
- **Database**: Microsoft SQL Server with Entity Framework Core 8.0.19
- **Authentication**: JWT Bearer Tokens, OAuth (Google, Facebook)
- **Real-time**: SignalR 1.2.0
- **Validation**: FluentValidation 12.0.0
- **Mapping**: AutoMapper 15.0.1
- **Email**: SMTP with Gmail
- **File Storage**: Amazon S3
- **AI Integration**: Google Gemini API
- **Documentation**: Swagger/OpenAPI 6.6.2
- **Password Hashing**: BCrypt.Net-Next 4.0.3

## 📁 Project Structure

```
backend/
├── Authentication/                    # Authentication system
│   ├── Implementation/               # Auth service implementations
│   │   ├── AuthService.cs           # Main authentication service
│   │   ├── FacebookAuthService.cs   # Facebook OAuth service
│   │   ├── GoogleAuthService.cs     # Google OAuth service
│   │   └── JwtService.cs            # JWT token management
│   └── Interfaces/                  # Authentication interfaces
├── Background/                      # Background jobs and services
│   ├── Jobs/                        # Scheduled tasks
│   └── Services/                    # Background services
├── Configuration/                   # Application configuration
│   ├── AuthConfiguration.cs         # Authentication setup
│   ├── CorsConfiguration.cs         # CORS policy configuration
│   ├── DatabaseConfiguration.cs     # Database setup
│   ├── JsonOptionsConfiguration.cs  # JSON serialization options
│   ├── ServicesConfiguration.cs     # Service registration
│   └── SwaggerConfiguration.cs      # API documentation setup
├── Controllers/                     # API Controllers
│   ├── AssignmentController.cs      # Assignment management
│   ├── AuthController.cs            # Authentication endpoints
│   ├── ChatController.cs            # Chat/messaging endpoints
│   ├── CourseController.cs          # Course management
│   ├── ForgotPasswordController.cs  # Password reset
│   └── ProfileController.cs         # User profile management
├── Data/                           # Data access layer
│   ├── Configurations/             # Entity configurations
│   └── DataContext.cs              # EF Core DbContext
├── Exceptions/                     # Exception handling
│   ├── CustomExceptions/           # Custom exception types
│   └── GlobalExceptionHandler.cs   # Global error handler
├── Hubs/                          # SignalR Hubs
│   └── ChatHub.cs                 # Real-time chat hub
├── Migrations/                    # Database migrations
├── Models/                        # Data models
│   ├── Auth/                      # Authentication models
│   ├── Common/                    # Shared models
│   ├── DTOs/                      # Data Transfer Objects
│   ├── Entities/                  # Database entities
│   ├── Enums/                     # Enumerations
│   ├── Requests/                  # Request models
│   ├── Responses/                 # Response models
│   ├── SignalR/                   # SignalR models
│   └── ValueObjects/              # Value objects
├── Security/                      # Security components
│   ├── Encryption/                # Encryption utilities
│   ├── Policies/                  # Authorization policies
│   └── Requirements/              # Custom authorization requirements
├── Services/                      # Business logic services
│   ├── Implementation/            # Service implementations
│   └── Interfaces/                # Service interfaces
├── SMTP/                          # Email services
│   ├── Implementation/            # Email service implementation
│   ├── Interfaces/                # Email interfaces
│   └── Templates/                 # Email templates
├── Utilities/                     # Utility classes
│   ├── Constants/                 # Application constants
│   ├── Extensions/                # Extension methods
│   ├── Helpers/                   # Helper classes
│   └── Mappings/                  # AutoMapper profiles
├── Validators/                    # FluentValidation validators
├── Program.cs                     # Application entry point
├── learnyx.csproj                 # Project file
└── appsettings.json              # Configuration file
```

## ✨ Features

### 🔐 Authentication & Authorization

- **JWT-based Authentication**: Secure token-based authentication
- **Multi-provider OAuth**: Google and Facebook OAuth integration
- **Role-based Access Control**: Student, Instructor, Admin roles
- **Password Management**: Secure password hashing with BCrypt
- **Password Reset**: Email-based password recovery

### 📚 Course Management

- **Course CRUD Operations**: Create, read, update, delete courses
- **Course Categories**: Organize courses by categories
- **Course Sections & Lessons**: Hierarchical course structure
- **Course Enrollments**: Student enrollment management
- **Course Reviews**: Student feedback and ratings
- **Progress Tracking**: Track student learning progress

### 📝 Assignment System

- **Assignment Creation**: Instructors can create assignments
- **Assignment Resources**: Attach files and resources
- **Student Submissions**: Submit assignments with files
- **Grading System**: Grade assignments and provide feedback
- **Submission Tracking**: Monitor submission status

### 💬 Real-time Communication

- **Live Chat**: Real-time messaging between users
- **Group Conversations**: Multi-user chat rooms
- **Message Status**: Read receipts and delivery status
- **User Presence**: Online/offline status tracking
- **Typing Indicators**: Real-time typing notifications
- **AI Assistant**: Integrated AI chat assistant

### 👤 User Management

- **Profile Management**: User profile CRUD operations
- **Avatar Upload**: Profile picture management
- **User Roles**: Dynamic role assignment
- **User Search**: Find and connect with users

### 📧 Email System

- **Transactional Emails**: Automated email notifications
- **Email Templates**: Customizable email templates
- **SMTP Integration**: Gmail SMTP configuration
- **Email Validation**: Email verification system

### 📁 File Management

- **AWS S3 Integration**: Scalable file storage
- **File Upload**: Secure file upload handling
- **File Types**: Support for various file formats
- **Access Control**: Secure file access management

### 🤖 AI Integration

- **Google Gemini**: AI-powered chat assistant
- **Context-aware Responses**: Maintains conversation context
- **Educational Support**: Course-related AI assistance

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK
- Microsoft SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code
- AWS Account (for S3 storage)
- Gmail Account (for SMTP)

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/GeorgeShani/Learnyx.git
   cd learnyx/backend
   ```

2. **Install dependencies**

   ```bash
   dotnet restore
   ```

3. **Configure the database**

   ```bash
   # Update connection string in appsettings.json
   # Run migrations
   dotnet ef database update
   ```

4. **Configure external services**

   - Update `appsettings.json` with your API keys
   - Configure AWS S3 credentials
   - Set up Gmail SMTP credentials
   - Configure OAuth applications

5. **Run the application**

   ```bash
   dotnet run
   ```

6. **Access the API**
   - API: `https://localhost:7188`
   - Swagger UI: `https://localhost:7188/swagger`

## ⚙️ Configuration

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=learnyx;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Authentication": {
    "Facebook": {
      "AppId": "your-facebook-app-id",
      "AppSecret": "your-facebook-app-secret",
      "RedirectUri": "https://localhost:7188/api/auth/facebook/callback"
    },
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret",
      "RedirectUri": "https://localhost:7188/api/auth/google/callback"
    }
  },
  "Jwt": {
    "Key": "your-jwt-secret-key",
    "Issuer": "gateway.learnyx.io",
    "Audience": "learnyx.io",
    "ExpiryHours": "72"
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "EnableSsl": true,
    "DisplayName": "Learnyx Support",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  },
  "AwsS3": {
    "AccessKey": "your-aws-access-key",
    "SecretKey": "your-aws-secret-key",
    "Region": "eu-north-1",
    "BucketName": "learnyx-storage-bucket"
  },
  "Gemini": {
    "ApiKey": "your-gemini-api-key"
  }
}
```

### Environment-specific Configuration

- **Development**: `appsettings.Development.json`
- **Production**: `appsettings.json`
- **Example**: `appsettings.Example.json` (template)

## 🏗 Architecture

### Clean Architecture Principles

The backend follows Clean Architecture principles with clear separation of concerns:

- **Controllers**: Handle HTTP requests and responses
- **Services**: Business logic implementation
- **Repositories**: Data access abstraction (via EF Core)
- **Models**: Data structures and DTOs
- **Configuration**: Application setup and configuration

### Dependency Injection

All services are registered using ASP.NET Core's built-in DI container:

```csharp
// Services registration
services.AddScoped<ICourseService, CourseService>();
services.AddScoped<IProfileService, ProfileService>();
services.AddScoped<IAssignmentService, AssignmentService>();
services.AddScoped<IChatService, ChatService>();
services.AddScoped<IEmailService, EmailService>();
services.AddScoped<IAmazonS3Service, AmazonS3Service>();
services.AddScoped<IGeminiService, GeminiService>();
```

### Middleware Pipeline

```csharp
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");
```

## 🔌 API Endpoints

### Authentication Endpoints

| Method | Endpoint                      | Description                   |
| ------ | ----------------------------- | ----------------------------- |
| POST   | `/api/auth/login`             | User login                    |
| POST   | `/api/auth/signup`            | User registration             |
| POST   | `/api/auth/google`            | Google OAuth authentication   |
| GET    | `/api/auth/google/callback`   | Google OAuth callback         |
| POST   | `/api/auth/facebook`          | Facebook OAuth authentication |
| GET    | `/api/auth/facebook/callback` | Facebook OAuth callback       |
| GET    | `/api/auth/me`                | Get authenticated user        |

### Course Endpoints

| Method | Endpoint                     | Description           |
| ------ | ---------------------------- | --------------------- |
| GET    | `/api/courses`               | Get all courses       |
| GET    | `/api/courses/{id}`          | Get course by ID      |
| POST   | `/api/courses`               | Create new course     |
| PUT    | `/api/courses/{id}`          | Update course         |
| DELETE | `/api/courses/{id}`          | Delete course         |
| POST   | `/api/courses/{id}/enroll`   | Enroll in course      |
| GET    | `/api/courses/{id}/students` | Get enrolled students |

### Assignment Endpoints

| Method | Endpoint                            | Description          |
| ------ | ----------------------------------- | -------------------- |
| GET    | `/api/assignments`                  | Get assignments      |
| GET    | `/api/assignments/{id}`             | Get assignment by ID |
| POST   | `/api/assignments`                  | Create assignment    |
| PUT    | `/api/assignments/{id}`             | Update assignment    |
| DELETE | `/api/assignments/{id}`             | Delete assignment    |
| POST   | `/api/assignments/{id}/submit`      | Submit assignment    |
| GET    | `/api/assignments/{id}/submissions` | Get submissions      |

### Chat Endpoints

| Method | Endpoint                                | Description               |
| ------ | --------------------------------------- | ------------------------- |
| GET    | `/api/chat/conversations`               | Get user conversations    |
| POST   | `/api/chat/conversations`               | Create conversation       |
| GET    | `/api/chat/conversations/{id}/messages` | Get conversation messages |
| POST   | `/api/chat/conversations/{id}/messages` | Send message              |
| PUT    | `/api/chat/messages/{id}/read`          | Mark message as read      |

### Profile Endpoints

| Method | Endpoint               | Description      |
| ------ | ---------------------- | ---------------- |
| GET    | `/api/profile`         | Get user profile |
| PUT    | `/api/profile`         | Update profile   |
| POST   | `/api/profile/avatar`  | Upload avatar    |
| GET    | `/api/profile/courses` | Get user courses |

## 🗄️ Database Schema

### Core Entities

#### User Entity

```csharp
public class User : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsEmailVerified { get; set; }
    public string? ResetPasswordToken { get; set; }
    public DateTime? ResetPasswordExpires { get; set; }
}
```

#### Course Entity

```csharp
public class Course : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public CourseStatus Status { get; set; }
    public int InstructorId { get; set; }
    public int CategoryId { get; set; }
    public User Instructor { get; set; }
    public CourseCategory Category { get; set; }
    public ICollection<CourseSection> Sections { get; set; }
    public ICollection<CourseEnrollment> Enrollments { get; set; }
}
```

#### Assignment Entity

```csharp
public class Assignment : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public int MaxPoints { get; set; }
    public AssignmentStatus Status { get; set; }
    public int CourseId { get; set; }
    public int InstructorId { get; set; }
    public Course Course { get; set; }
    public User Instructor { get; set; }
    public ICollection<Submission> Submissions { get; set; }
}
```

### Entity Relationships

- **User** ↔ **Course**: One-to-Many (Instructor)
- **User** ↔ **CourseEnrollment**: One-to-Many (Student)
- **Course** ↔ **CourseSection**: One-to-Many
- **CourseSection** ↔ **Lesson**: One-to-Many
- **Course** ↔ **Assignment**: One-to-Many
- **Assignment** ↔ **Submission**: One-to-Many
- **User** ↔ **Conversation**: Many-to-Many
- **Conversation** ↔ **Message**: One-to-Many

## 🔐 Authentication & Authorization

### JWT Configuration

```csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
```

### Role-based Authorization

```csharp
[Authorize(Roles = "Instructor,Admin")]
public async Task<IActionResult> CreateAssignment([FromBody] CreateAssignmentRequest request)
{
    // Implementation
}
```

### OAuth Integration

#### Google OAuth

- Client ID and Secret configuration
- Redirect URI handling
- User profile extraction
- Account linking

#### Facebook OAuth

- App ID and Secret configuration
- Permission scopes (email, public_profile)
- User data retrieval
- Profile picture handling

## ⚡ Real-time Features

### SignalR Chat Hub

The `ChatHub` provides real-time communication features:

#### Connection Management

```csharp
public override async Task OnConnectedAsync()
{
    var userId = GetCurrentUserId();
    // Track user presence
    // Notify other users
}
```

#### Message Broadcasting

```csharp
public async Task SendMessage(int conversationId, string textContent, List<MessageContentDTO> contents)
{
    var message = await _chatService.SendMessageAsync(conversationId, userId, textContent, contents);
    await Clients.Group($"conversation_{conversationId}")
        .SendAsync("ReceiveMessage", message);
}
```

#### User Presence

- Online/offline status tracking
- Last seen timestamps
- Connection count management
- Real-time presence updates

#### Typing Indicators

- Start/stop typing notifications
- Real-time typing status
- User activity tracking

### AI Assistant Integration

```csharp
private async Task HandleAssistantResponse(int conversationId, int userId)
{
    // Show typing indicator
    await Clients.Group($"conversation_{conversationId}")
        .SendAsync("AssistantTyping", true);

    // Get AI response
    var response = await _chatService.GetAssistantResponseAsync(conversationId);

    // Send response
    var assistantMessage = await _chatService.SendAssistantMessageAsync(conversationId, response);
    await Clients.Group($"conversation_{conversationId}")
        .SendAsync("ReceiveMessage", assistantMessage);
}
```

## 🔗 External Integrations

### Amazon S3 File Storage

```csharp
public class AmazonS3Service : IAmazonS3Service
{
    public async Task<string> UploadFileAsync(IFormFile file, string folder)
    {
        using var client = new AmazonS3Client(_awsSettings.AccessKey, _awsSettings.SecretKey, RegionEndpoint.GetBySystemName(_awsSettings.Region));

        var key = $"{folder}/{Guid.NewGuid()}_{file.FileName}";
        var request = new PutObjectRequest
        {
            BucketName = _awsSettings.BucketName,
            Key = key,
            InputStream = file.OpenReadStream(),
            ContentType = file.ContentType
        };

        await client.PutObjectAsync(request);
        return $"https://{_awsSettings.BucketName}.s3.{_awsSettings.Region}.amazonaws.com/{key}";
    }
}
```

### Google Gemini AI

```csharp
public class GeminiService : IGeminiService
{
    public async Task<string> GenerateResponseAsync(string prompt, string context)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("x-goog-api-key", _geminiSettings.ApiKey);

        var request = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = $"{context}\n\n{prompt}" }
                    }
                }
            }
        };

        var response = await client.PostAsJsonAsync("https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent", request);
        // Process response and return generated text
    }
}
```

### SMTP Email Service

```csharp
public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port);
        client.EnableSsl = _smtpSettings.EnableSsl;
        client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);

        var message = new MailMessage
        {
            From = new MailAddress(_smtpSettings.Username, _smtpSettings.DisplayName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(to);
        await client.SendMailAsync(message);
    }
}
```

## 🛠 Development

### Code Generation

Use Entity Framework CLI for database operations:

```bash
# Add new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# Generate new entity
dotnet ef dbcontext scaffold "ConnectionString" Microsoft.EntityFrameworkCore.SqlServer
```

### Validation

FluentValidation is used for request validation:

```csharp
public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative");
    }
}
```

### Error Handling

Global exception handling with custom exceptions:

```csharp
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var response = exception switch
        {
            ValidationException ex => new { Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) },
            NotFoundException ex => new { Message = ex.Message },
            UnauthorizedException ex => new { Message = ex.Message },
            _ => new { Message = "An unexpected error occurred" }
        };

        httpContext.Response.StatusCode = exception switch
        {
            ValidationException => 400,
            NotFoundException => 404,
            UnauthorizedException => 401,
            _ => 500
        };

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
```

## 🧪 Testing

### Unit Testing

```bash
# Run unit tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test
dotnet test --filter "ClassName=AuthServiceTests"
```

### Integration Testing

```csharp
public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<AuthResponse>();
        content.Should().NotBeNull();
        content.Token.Should().NotBeNullOrEmpty();
    }
}
```

## 🚀 Deployment

### Docker Deployment

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["learnyx.csproj", "."]
RUN dotnet restore "learnyx.csproj"
COPY . .
RUN dotnet build "learnyx.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "learnyx.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "learnyx.dll"]
```

### Environment Variables

Set the following environment variables for production:

```bash
# Database
ConnectionStrings__DefaultConnection="Server=prod-server;Database=learnyx;..."

# JWT
Jwt__Key="your-production-jwt-key"
Jwt__Issuer="learnyx.io"
Jwt__Audience="learnyx.io"

# OAuth
Authentication__Google__ClientId="your-production-google-client-id"
Authentication__Google__ClientSecret="your-production-google-client-secret"

# AWS
AwsS3__AccessKey="your-production-aws-access-key"
AwsS3__SecretKey="your-production-aws-secret-key"
AwsS3__BucketName="learnyx-production-bucket"

# SMTP
Smtp__Username="your-production-email"
Smtp__Password="your-production-app-password"

# Gemini
Gemini__ApiKey="your-production-gemini-api-key"
```

### Production Considerations

- **HTTPS**: Ensure HTTPS is enabled in production
- **CORS**: Configure CORS for your frontend domain
- **Rate Limiting**: Implement rate limiting for API endpoints
- **Logging**: Configure structured logging with Serilog
- **Monitoring**: Set up application monitoring and health checks
- **Backup**: Implement database backup strategies
- **Scaling**: Consider horizontal scaling with load balancers

## 📚 API Documentation

The API documentation is automatically generated using Swagger/OpenAPI and is available at:

- **Development**: `https://localhost:7188/swagger`
- **Production**: `https://your-domain.com/swagger`

### Swagger Configuration

```csharp
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Learnyx API",
        Version = "v1",
        Description = "Learning Management System API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
});
```

## 🔧 Troubleshooting

### Common Issues

1. **Database Connection Issues**

   - Verify connection string
   - Ensure SQL Server is running
   - Check firewall settings

2. **Authentication Problems**

   - Verify JWT configuration
   - Check OAuth credentials
   - Validate token expiration

3. **File Upload Issues**

   - Check AWS S3 credentials
   - Verify bucket permissions
   - Ensure file size limits

4. **Email Delivery Problems**
   - Verify SMTP credentials
   - Check Gmail app password
   - Ensure network connectivity

### Logging

Enable detailed logging for debugging:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "learnyx": "Debug"
    }
  }
}
```

## 🤝 Contributing

1. Follow C# coding conventions
2. Write unit tests for new features
3. Update API documentation
4. Use meaningful commit messages
5. Test with different user roles
6. Ensure backward compatibility

## 📄 License

This project is part of the Learnyx Learning Management System.

---

For more information, visit the [API Documentation](https://localhost:7188/swagger) or contact the development team.
