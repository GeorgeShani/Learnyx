using System.Text;
using learnyx.Models.Enums;
using System.Security.Claims;
using learnyx.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using learnyx.Authentication.Interfaces;

namespace learnyx.Authentication.Implementation;

 public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _tokenExpirationHours;

    public JwtService(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:Key"]!;
        _issuer = configuration["Jwt:Issuer"]!;
        _audience = configuration["Jwt:Audience"]!;
        _tokenExpirationHours = int.Parse(configuration["Jwt:ExpiryHours"]!);
    }

    public string GenerateToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("auth_provider", user.AuthProvider),
        };

        // Add provider-specific claims
        if (!string.IsNullOrEmpty(user.GoogleId))
            claims.Add(new Claim("google_id", user.GoogleId));

        if (!string.IsNullOrEmpty(user.FacebookId))        
            claims.Add(new Claim("facebook_id", user.FacebookId));


        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_tokenExpirationHours),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            return validatedToken is JwtSecurityToken;
        }
        catch
        {
            return false;
        }
    }

    public int? GetUserIdFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            
            var userIdClaim = jsonToken?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public string? GetUserEmailFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            
            return jsonToken?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        }
        catch
        {
            return null;
        }
    }

    public UserRole? GetUserRoleFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            
            var roleClaim = jsonToken?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Role);
            if (roleClaim != null && Enum.TryParse<UserRole>(roleClaim.Value, out var role))
                return role;
            
            return null;
        }
        catch
        {
            return null;
        }
    }
}
