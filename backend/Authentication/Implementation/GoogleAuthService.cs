using learnyx.Data;
using Google.Apis.Auth;
using learnyx.Models.Enums;
using Google.Apis.Services;
using Google.Apis.Oauth2.v2;
using learnyx.Models.Entities;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.EntityFrameworkCore;
using learnyx.Authentication.Interfaces;
using learnyx.SMTP.Interfaces;
using learnyx.SMTP.Templates;

namespace learnyx.Authentication.Implementation;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEmailService _emailService;
    private readonly ILogger<GoogleAuthService> _logger;
    
    public GoogleAuthService(
        IConfiguration configuration, 
        IServiceProvider serviceProvider, 
        IEmailService emailService,
        ILogger<GoogleAuthService> logger
    ) {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _emailService = emailService;
        _logger = logger;
    }
    
    public async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string idToken)
    {
        try
        {
            var googleClientId = _configuration["Authentication:Google:ClientId"]!;
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { googleClientId }
            };
            
            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            return payload;
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException("Invalid Google token", ex);
        }
    }
    
    public async Task<User> AuthenticateGoogleUserAsync(string idToken)
    {
        try
        {
            var payload = await ValidateGoogleTokenAsync(idToken);
            
            var email = payload.Email;
            var firstName = payload.GivenName ?? "";
            var lastName = payload.FamilyName ?? "";
            var googleId = payload.Subject; // This is the unique Google user ID
            var profilePicture = payload.Picture;
            
            var user = await FindOrCreateGoogleUserAsync(email, firstName, lastName, googleId, profilePicture);
            return user;
        }
        catch (UnauthorizedAccessException)
        {
            throw; // Re-throw validation errors
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating Google user");
            throw new InvalidOperationException("Failed to authenticate user with Google", ex);
        }
    }

    public async Task<User> AuthenticateWithCodeAsync(string authorizationCode)
    {
        try
        {
            var googleClientId = _configuration["Authentication:Google:ClientId"]!;
            var googleClientSecret = _configuration["Authentication:Google:ClientSecret"]!;
            var redirectUri = _configuration["Authentication:Google:RedirectUri"]!;

            // Create OAuth2 flow
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = googleClientId,
                    ClientSecret = googleClientSecret
                },
                Scopes = new[] { "openid", "email", "profile" }
            });

            // Exchange authorization code for tokens
            var tokenResponse = await flow.ExchangeCodeForTokenAsync("user", authorizationCode, redirectUri, CancellationToken.None);

            // Get user information using the access token
            var userInfo = await GetGoogleUserInfoAsync(tokenResponse.AccessToken);

            var user = await FindOrCreateGoogleUserAsync(
                userInfo.Email,
                userInfo.GivenName ?? "",
                userInfo.FamilyName ?? "",
                userInfo.Id,
                userInfo.Picture
            );

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating with Google authorization code");
            throw new UnauthorizedAccessException("Failed to authenticate with Google authorization code", ex);
        }
    }

    private async Task<Userinfo> GetGoogleUserInfoAsync(string accessToken)
    {
        try
        {
            var oauth2Service = new Oauth2Service(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromAccessToken(accessToken)
            });

            var userInfoRequest = oauth2Service.Userinfo.Get();
            var userInfo = await userInfoRequest.ExecuteAsync();
            
            return userInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving Google user info");
            throw new UnauthorizedAccessException("Failed to retrieve user information from Google", ex);
        }
    }

    private async Task<User> FindOrCreateGoogleUserAsync(
        string email, string firstName, string lastName, 
        string googleId, string? profilePicture
    ) {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);
        if (existingUser != null)
        {
            // Update user info if needed
            await UpdateUserInfoIfNeededAsync(context, existingUser, email, firstName, lastName, profilePicture);
            return existingUser;
        }

        // If not found by Google ID, try to find by email
        existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (existingUser != null)
        {
            // Link Google account to existing user
            existingUser.GoogleId = googleId;
            if (!string.IsNullOrEmpty(profilePicture) && string.IsNullOrEmpty(existingUser.Avatar))
                existingUser.Avatar = profilePicture;
            
            await context.SaveChangesAsync();
            _logger.LogInformation("Linked Google account to existing user: {Email}", email);
            return existingUser;
        }
        
        var newUser = new User
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = UserRole.Student, // Default role - you might want to make this configurable
            AuthProvider = "Google",
            GoogleId = googleId,
            Avatar = profilePicture
        };

        context.Users.Add(newUser);
        await context.SaveChangesAsync();
        await _emailService.SendEmailAsync(
            newUser.Email,
            "Welcome to Learnyx!",
            EmailTemplates.GetWelcomeEmailTemplate(newUser.FirstName)
        );
        
        _logger.LogInformation("Created new Google user: {Email}", email);
        return newUser;
    }

    private async Task UpdateUserInfoIfNeededAsync(
        DataContext context, User user, string email, 
        string firstName, string lastName, string? profilePicture
    ) {
        var updated = false;

        // Update email if changed
        if (user.Email != email)
        {
            user.Email = email;
            updated = true;
        }

        if (string.IsNullOrEmpty(user.FirstName) || user.FirstName != firstName)
        {
            user.FirstName = firstName;
            updated = true;
        }

        if (string.IsNullOrEmpty(user.LastName) || user.LastName != lastName)
        {
            user.LastName = lastName;
            updated = true;
        }

        if (string.IsNullOrEmpty(user.Avatar) && !string.IsNullOrEmpty(profilePicture))
        {
            user.Avatar = profilePicture;
            updated = true;
        }

        if (updated)
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("Updated Google user info: {Email}", email);
        }
    }
}