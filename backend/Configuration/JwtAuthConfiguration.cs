using System.Text;
using learnyx.Authentication.Implementation;
using learnyx.Authentication.Interfaces;
using learnyx.Models.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace learnyx.Configuration;

public static class JwtAuthConfiguration
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Register JWT service
        services.AddScoped<IJwtService, JwtService>();

        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["Key"]!;
        var issuer = jwtSettings["Issuer"]!;
        var audience = jwtSettings["Audience"]!;

        var key = Encoding.UTF8.GetBytes(secretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false; // Set to true in production
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            // Handle JWT events
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    // Allow token to be passed via query string for SignalR
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    
                    if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    {
                        context.Token = accessToken;
                    }
                    
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(options =>
        {
            // Add role-based policies
            options.AddPolicy("AdminPolicy", policy => policy.RequireRole(nameof(UserRole.ADMIN)));
            options.AddPolicy("TeacherPolicy", policy => policy.RequireRole(nameof(UserRole.TEACHER), nameof(UserRole.ADMIN)));
            options.AddPolicy("StudentPolicy", policy => policy.RequireRole(nameof(UserRole.STUDENT), nameof(UserRole.TEACHER), nameof(UserRole.ADMIN)));
            
            // Add custom policies
            options.AddPolicy("ActiveUser", policy =>
                policy.RequireClaim("is_active", "True"));
            
            options.AddPolicy("EmailConfirmed", policy =>
                policy.RequireClaim("is_email_confirmed", "True"));
            
            options.AddPolicy("LocalAuth", policy =>
                policy.RequireClaim("auth_provider", "Local"));
            
            options.AddPolicy("SocialAuth", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim("auth_provider", "Google") ||
                    context.User.HasClaim("auth_provider", "Facebook")));
        });

        return services;
    }   
}