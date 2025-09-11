using learnyx.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using learnyx.Authentication.Interfaces;

namespace learnyx.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IFacebookAuthService _facebookAuthService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IGoogleAuthService googleAuthService, 
        IFacebookAuthService facebookAuthService,
        IJwtService jwtService, 
        ILogger<AuthController> logger
    ) {
        _googleAuthService = googleAuthService;
        _facebookAuthService = facebookAuthService;
        _jwtService = jwtService;
        _logger = logger;
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthRequest request)
    {
        if (string.IsNullOrEmpty(request.IdToken))
            return BadRequest(new { message = "Google ID token is required" });

        try
        {
            var user = await _googleAuthService.AuthenticateGoogleUserAsync(request.IdToken);
            var jwtToken = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token = jwtToken,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    role = user.Role.ToString(),
                    avatar = user.Avatar
                }
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Invalid Google token: {Error}", ex.Message);
            return Unauthorized(new { message = "Invalid Google token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google authentication");
            return StatusCode(500, new { message = "Authentication failed" });
        }
    }

    [HttpPost("facebook")]
    public async Task<IActionResult> FacebookAuth([FromBody] FacebookAuthRequest request)
    {
        if (string.IsNullOrEmpty(request.AccessToken))
        {
            return BadRequest(new { message = "Facebook access token is required" });
        }

        try
        {
            var user = await _facebookAuthService.AuthenticateAsync(request.AccessToken);
            
            if (string.IsNullOrEmpty(user.Email))
            {
                return Ok(new 
                { 
                    requiresEmail = true,
                    userId = user.Id,
                    message = "Please provide your email address to complete registration"
                });
            }
        
            // Generate JWT token
            var jwtToken = _jwtService.GenerateToken(user);
        
            return Ok(new 
            { 
                token = jwtToken,
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    role = user.Role.ToString(),
                    avatar = user.Avatar
                }
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Invalid Facebook token: {Error}", ex.Message);
            return Unauthorized(new { message = "Invalid Facebook access token" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Facebook authentication");
            return StatusCode(500, new { message = "Authentication failed" });
        }
    }
}