using System.Text.Json;
using learnyx.Models.Auth;
using learnyx.Models.Entities;
using learnyx.Authentication.Interfaces;
using learnyx.Data;
using learnyx.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace learnyx.Authentication.Implementation;

public class FacebookAuthService : IFacebookAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FacebookAuthService> _logger;
    
    private const string FacebookGraphApiUrl = "https://graph.facebook.com/v18.0";
    private const string FacebookUserInfoFields = "id,email,first_name,last_name,name,picture.type(large)";

    public FacebookAuthService(
        HttpClient httpClient, 
        IConfiguration configuration, 
        IServiceProvider serviceProvider, 
        ILogger<FacebookAuthService> logger
    ) {
        _httpClient = httpClient;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<bool> ValidateAccessTokenAsync(string accessToken)
    {
        try
        {
            var appId = _configuration["Authentication:Facebook:AppId"];
            var appSecret = _configuration["Authentication:Facebook:AppSecret"];
            
            var url = $"{FacebookGraphApiUrl}/debug_token?input_token={accessToken}&access_token={appId}|{appSecret}";
            
            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Facebook token validation failed: {Response}", json);
                return false;
            }

            var validationResponse = JsonSerializer.Deserialize<FacebookTokenValidationResponse>(json, 
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

            var isValid = validationResponse?.Data?.IsValid == true && 
                         validationResponse.Data.AppId == appId;
                         
            if (!isValid)
                _logger.LogWarning("Facebook access token validation failed for token");
            
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Facebook access token");
            return false;
        }
    }

    public async Task<FacebookUserInfo> GetUserInfoAsync(string accessToken)
    {
        try
        {
            var isValidToken = await ValidateAccessTokenAsync(accessToken);
            if (!isValidToken)
                throw new UnauthorizedAccessException("Invalid Facebook access token");

            var url = $"{FacebookGraphApiUrl}/me?fields={FacebookUserInfoFields}&access_token={accessToken}";
            
            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get Facebook user info: {Response}", json);
                throw new InvalidOperationException($"Failed to retrieve user information from Facebook: {response.StatusCode}");
            }

            var userInfo = JsonSerializer.Deserialize<FacebookUserInfo>(json, 
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

            if (userInfo == null)
                throw new InvalidOperationException("Failed to deserialize Facebook user information");

            // Validate that we have essential information
            if (string.IsNullOrEmpty(userInfo.Id))
                throw new InvalidOperationException("Facebook user ID is missing");

            _logger.LogInformation("Successfully retrieved Facebook user info for user ID: {UserId}", userInfo.Id);
            return userInfo;
        }
        catch (UnauthorizedAccessException)
        {
            throw; // Re-throw authorization errors
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Facebook user info");
            throw new InvalidOperationException("Failed to retrieve user information from Facebook", ex);
        }
    }

    public async Task<User> AuthenticateAsync(string accessToken)
    {
        try
        {
            // Get user information from Facebook
            var facebookUser = await GetUserInfoAsync(accessToken);
            
            var user = await FindOrCreateFacebookUserAsync(facebookUser);
            
            _logger.LogInformation("Successfully authenticated Facebook user: {Email}", user.Email);
            return user;
        }
        catch (UnauthorizedAccessException)
        {
            throw; // Re-throw validation errors
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating Facebook user");
            throw new InvalidOperationException("Failed to authenticate user with Facebook", ex);
        }
    }

    public async Task<User> AuthenticateWithCodeAsync(string authorizationCode)
    {
        try
        {
            // Step 1: Exchange authorization code for access token
            var accessToken = await ExchangeCodeForTokenAsync(authorizationCode);
            
            // Step 2: Get user information using the access token
            var facebookUser = await GetFacebookUserInfoWithTokenAsync(accessToken);
            
            // Step 3: Find or create user
            var user = await FindOrCreateFacebookUserAsync(facebookUser);
            
            _logger.LogInformation("Successfully authenticated Facebook user with code: {Email}", user.Email);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating with Facebook authorization code");
            throw new UnauthorizedAccessException("Failed to authenticate with Facebook authorization code", ex);
        }
    }

    private async Task<string> ExchangeCodeForTokenAsync(string authorizationCode)
    {
        try
        {
            var appId = _configuration["Authentication:Facebook:AppId"]!;
            var appSecret = _configuration["Authentication:Facebook:AppSecret"]!;
            var redirectUri = _configuration["Authentication:Facebook:RedirectUri"]!;

            var tokenUrl = $"{FacebookGraphApiUrl}/oauth/access_token" +
                          $"?client_id={appId}" +
                          $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                          $"&client_secret={appSecret}" +
                          $"&code={authorizationCode}";

            var response = await _httpClient.GetAsync(tokenUrl);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to exchange Facebook code for token: {Response}", json);
                throw new UnauthorizedAccessException("Failed to exchange authorization code for access token");
            }

            var tokenResponse = JsonSerializer.Deserialize<FacebookTokenResponse>(json,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

            if (tokenResponse?.AccessToken == null)
            {
                _logger.LogError("Facebook token response missing access token: {Response}", json);
                throw new UnauthorizedAccessException("Invalid token response from Facebook");
            }

            return tokenResponse.AccessToken;
        }
        catch (Exception ex) when (!(ex is UnauthorizedAccessException))
        {
            _logger.LogError(ex, "Error exchanging Facebook authorization code for token");
            throw new UnauthorizedAccessException("Failed to exchange authorization code for access token", ex);
        }
    }

    private async Task<FacebookUserInfo> GetFacebookUserInfoWithTokenAsync(string accessToken)
    {
        try
        {
            var url = $"{FacebookGraphApiUrl}/me?fields={FacebookUserInfoFields}&access_token={accessToken}";
            
            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to get Facebook user info with token: {Response}", json);
                throw new UnauthorizedAccessException("Failed to retrieve user information from Facebook");
            }

            var userInfo = JsonSerializer.Deserialize<FacebookUserInfo>(json, 
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower });

            if (userInfo == null || string.IsNullOrEmpty(userInfo.Id))
            {
                throw new UnauthorizedAccessException("Invalid user information received from Facebook");
            }

            return userInfo;
        }
        catch (Exception ex) when (ex is not UnauthorizedAccessException)
        {
            _logger.LogError(ex, "Error getting Facebook user info with token");
            throw new UnauthorizedAccessException("Failed to retrieve user information from Facebook", ex);
        }
    }

    private async Task<User> FindOrCreateFacebookUserAsync(FacebookUserInfo facebookUser)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        
        var existingUser = await context.Users.FirstOrDefaultAsync(u => u.FacebookId == facebookUser.Id);
        if (existingUser != null)
        {
            await UpdateUserInfoIfNeededAsync(context, existingUser, facebookUser);
            return existingUser;
        }

        if (!string.IsNullOrEmpty(facebookUser.Email))
        {
            existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == facebookUser.Email);
            if (existingUser != null)
            {
                existingUser.FacebookId = facebookUser.Id;
                if (string.IsNullOrEmpty(existingUser.Avatar) && facebookUser.Picture?.Data?.Url != null)
                    existingUser.Avatar = facebookUser.Picture.Data.Url;
                
                await context.SaveChangesAsync();
                _logger.LogInformation("Linked Facebook account to existing user: {Email}", facebookUser.Email);
                return existingUser;
            }
        }
        
        var newUser = new User
        {
            Email = facebookUser.Email,
            FirstName = facebookUser.FirstName,
            LastName = facebookUser.LastName,
            Role = UserRole.STUDENT,
            AuthProvider = "Facebook",
            FacebookId = facebookUser.Id,
            Avatar = facebookUser.Picture?.Data?.Url
        };
        
        if (string.IsNullOrEmpty(newUser.Email))
            _logger.LogWarning("Facebook user {FacebookId} has no email address", facebookUser.Id);

        context.Users.Add(newUser);
        await context.SaveChangesAsync();
        
        _logger.LogInformation("Created new Facebook user: {FacebookId}, Email: {Email}", facebookUser.Id, newUser.Email);
        return newUser;
    }

    private async Task UpdateUserInfoIfNeededAsync(DataContext context, User user, FacebookUserInfo facebookUser)
    {
        var updated = false;
        
        if (string.IsNullOrEmpty(user.Email) && !string.IsNullOrEmpty(facebookUser.Email))
        {
            user.Email = facebookUser.Email;
            updated = true;
        }
        
        if (string.IsNullOrEmpty(user.FirstName) || user.FirstName != facebookUser.FirstName)
        {
            user.FirstName = facebookUser.FirstName;
            updated = true;
        }
        
        if (string.IsNullOrEmpty(user.LastName) || user.LastName != facebookUser.LastName)
        {
            user.LastName = facebookUser.LastName;
            updated = true;
        }

        if (string.IsNullOrEmpty(user.Avatar) && facebookUser.Picture?.Data?.Url != null)
        {
            user.Avatar = facebookUser.Picture.Data.Url;
            updated = true;
        }

        if (updated)
        {
            await context.SaveChangesAsync();
            _logger.LogInformation("Updated Facebook user info: {FacebookId}", facebookUser.Id);
        }
    }
}