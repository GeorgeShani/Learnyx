using FluentValidation;
using learnyx.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using learnyx.Authentication.Interfaces;

namespace learnyx.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IGoogleAuthService _googleAuthService;
    private readonly IFacebookAuthService _facebookAuthService;
    private readonly ILogger<AuthController> _logger;
    private readonly IJwtService _jwtService;

    public AuthController(
        IAuthService authService,
        IGoogleAuthService googleAuthService, 
        IFacebookAuthService facebookAuthService,
        IJwtService jwtService, 
        ILogger<AuthController> logger
    ) {
        _authService = authService;
        _googleAuthService = googleAuthService;
        _facebookAuthService = facebookAuthService;
        _jwtService = jwtService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.Login(request);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Internal Server Error", Details = ex.Message });
        }
    }

    [HttpPost("signup")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var result = await _authService.Register(request);
            return Created("", result);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new
            {
                Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
            });
        }
        catch (Exception ex)
        {
            // For logging and generic 500 response
            return StatusCode(500, new { Message = "Internal Server Error", Details = ex.Message });
        }
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

    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string? state, [FromQuery] string? error)
    {
        // Handle error cases
        if (!string.IsNullOrEmpty(error))
        {
            _logger.LogWarning("Google OAuth error: {Error}", error);
            return BadRequest(new { message = $"Google OAuth error: {error}" });
        }

        if (string.IsNullOrEmpty(code))
        {
            return BadRequest(new { message = "Authorization code is required" });
        }

        try
        {
            var user = await _googleAuthService.AuthenticateWithCodeAsync(code);
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
            _logger.LogWarning("Invalid Google authorization code: {Error}", ex.Message);
            return Unauthorized(new { message = "Invalid Google authorization code" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Google OAuth callback");
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
    
    [HttpGet("facebook/callback")]
    public async Task<IActionResult> FacebookOAuthCallback([FromQuery] string code, [FromQuery] string? state, [FromQuery] string? error)
    {
        // Handle error cases
        if (!string.IsNullOrEmpty(error))
        {
            _logger.LogWarning("Facebook OAuth error: {Error}", error);
            return BadRequest(new { message = $"Facebook OAuth error: {error}" });
        }

        if (string.IsNullOrEmpty(code))
        {
            return BadRequest(new { message = "Authorization code is required" });
        }

        try
        {
            var user = await _facebookAuthService.AuthenticateWithCodeAsync(code);
            
            if (string.IsNullOrEmpty(user.Email))
            {
                return Ok(new 
                { 
                    requiresEmail = true,
                    userId = user.Id,
                    message = "Please provide your email address to complete registration"
                });
            }
            
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
            _logger.LogWarning("Invalid Facebook authorization code: {Error}", ex.Message);
            return Unauthorized(new { message = "Invalid Facebook authorization code" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Facebook OAuth callback");
            return StatusCode(500, new { message = "Authentication failed" });
        }
    }
}