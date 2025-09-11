using learnyx.Data;
using Google.Apis.Auth;
using learnyx.Models.Enums;
using learnyx.Models.Entities;
using Microsoft.EntityFrameworkCore;
using learnyx.Authentication.Interfaces;

namespace learnyx.Authentication.Implementation;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<GoogleAuthService> _logger;
    
    public GoogleAuthService(
        IConfiguration configuration, 
        IServiceProvider serviceProvider, 
        ILogger<GoogleAuthService> logger
    ) {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
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
            Role = UserRole.STUDENT, // Default role - you might want to make this configurable
            AuthProvider = "Google",
            GoogleId = googleId,
            Avatar = profilePicture
        };

        context.Users.Add(newUser);
        await context.SaveChangesAsync();
        
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