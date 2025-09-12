using System.Text;
using learnyx.Models.Enums;
using Microsoft.IdentityModel.Tokens;
using learnyx.Authentication.Interfaces;
using learnyx.Authentication.Implementation;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace learnyx.Configuration;

public static class AuthConfiguration
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Register JWT service
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();
        services.AddScoped<IFacebookAuthService, FacebookAuthService>();
        services.AddHttpClient<IFacebookAuthService, FacebookAuthService>();

        var jwtSettings = configuration.GetSection("Jwt");
        var secretKey = jwtSettings["Key"]!;
        var issuer = jwtSettings["Issuer"]!;
        var audience = jwtSettings["Audience"]!;
        
        var googleSettings = configuration.GetSection("Authentication:Google");
        var googleClientId = googleSettings["ClientId"]!;
        var googleClientSecret = googleSettings["ClientSecret"]!;
        
        var facebookSettings = configuration.GetSection("Authentication:Facebook");
        var facebookAppId = facebookSettings["AppId"]!;
        var facebookAppSecret = facebookSettings["AppSecret"]!;

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
        })
        .AddCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.None; // allow cross-site
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        })
        .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
        {
            options.ClientId = googleClientId;
            options.ClientSecret = googleClientSecret;
            options.SaveTokens = true;
                
            // Request additional scopes if needed
            options.Scope.Add("email");
            options.Scope.Add("profile");
                
            // Configure a callback path
            options.CallbackPath = "/signin-google";
        })
        .AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
        {
            options.AppId = facebookAppId;
            options.AppSecret = facebookAppSecret;
            options.SaveTokens = true;
            
            // Request additional scopes if needed
            options.Scope.Add("email");
            options.Scope.Add("public_profile");
            
            // Configure a callback path
            options.CallbackPath = "/signin-facebook";
        });

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminPolicy", 
                policy => policy.RequireRole(nameof(UserRole.ADMIN))
            )
            .AddPolicy("TeacherPolicy", 
                policy => policy.RequireRole(
                    nameof(UserRole.TEACHER), 
                    nameof(UserRole.ADMIN)
                )
            )
            .AddPolicy("StudentPolicy",
                policy => policy.RequireRole(
                    nameof(UserRole.STUDENT),
                    nameof(UserRole.TEACHER),
                    nameof(UserRole.ADMIN)
                )
            )
            .AddPolicy("LocalAuth", policy =>
                policy.RequireClaim("auth_provider", "Local")
            )
            .AddPolicy("SocialAuth", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim("auth_provider", "Google") ||
                    context.User.HasClaim("auth_provider", "Facebook")
                )
            );

        return services;
    }   
}