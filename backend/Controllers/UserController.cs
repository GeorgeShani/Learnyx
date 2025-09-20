using learnyx.Models.Entities;
using learnyx.Models.Enums;
using learnyx.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace learnyx.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAmazonS3Service _s3Service;

    public UserController(IUserService userService, IAmazonS3Service s3Service)
    {
        _userService = userService;
        _s3Service = s3Service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound($"User with ID {id} not found.");

        return Ok(user);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<User>> GetUserByEmail(string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null)
            return NotFound($"User with email {email} not found.");

        return Ok(user);
    }

    [HttpGet("role/{role}")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(UserRole role)
    {
        var users = await _userService.GetUsersByRoleAsync(role);
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        // Check if email already exists
        if (await _userService.EmailExistsAsync(user.Email))
        {
            return BadRequest($"User with email {user.Email} already exists.");
        }

        var createdUser = await _userService.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<User>> UpdateUser(int id, User user)
    {
        if (!await _userService.UserExistsAsync(id))
        {
            return NotFound($"User with ID {id} not found.");
        }

        var existingUser = await _userService.GetUserByIdAsync(id);
        if (existingUser != null && existingUser.Email != user.Email && await _userService.EmailExistsAsync(user.Email))
        {
            return BadRequest($"User with email {user.Email} already exists.");
        }

        var updatedUser = await _userService.UpdateUserAsync(id, user);
        if (updatedUser == null)
            return NotFound($"User with ID {id} not found.");

        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result)
            return NotFound($"User with ID {id} not found.");

        return NoContent();
    }

    [HttpGet("check-email/{email}")]
    public async Task<ActionResult<bool>> CheckEmailExists(string email)
    {
        var exists = await _userService.EmailExistsAsync(email);
        return Ok(new { exists });
    }

    // OAuth-specific endpoints
    [HttpGet("google/{googleId}")]
    public async Task<ActionResult<User>> GetUserByGoogleId(string googleId)
    {
        var user = await _userService.GetUserByGoogleIdAsync(googleId);
        if (user == null)
            return NotFound($"User with Google ID {googleId} not found.");

        return Ok(user);
    }

    [HttpGet("facebook/{facebookId}")]
    public async Task<ActionResult<User>> GetUserByFacebookId(string facebookId)
    {
        var user = await _userService.GetUserByFacebookIdAsync(facebookId);
        if (user == null)
            return NotFound($"User with Facebook ID {facebookId} not found.");

        return Ok(user);
    }
    
    [HttpPost("{id}/avatar")]
    public async Task<ActionResult> UploadAvatar(int id, IFormFile avatar)
    {
        // Validate user exists
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound($"User with ID {id} not found.");

        // Validate file
        if (avatar.Length == 0)
            return BadRequest("No file uploaded.");

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(avatar.ContentType.ToLower()))
            return BadRequest("Invalid file type. Only JPEG, PNG, GIF, and WebP images are allowed.");

        // Validate file size (max 5MB)
        const long maxFileSize = 5 * 1024 * 1024; // 5MB
        if (avatar.Length > maxFileSize)
            return BadRequest("File size cannot exceed 5MB.");

        try
        {
            // Delete existing avatar if present
            if (!string.IsNullOrEmpty(user.Avatar))
            {
                try
                {
                    await _s3Service.DeleteImageFromS3(user.Avatar);
                }
                catch (Exception ex)
                {
                    // Log but don't fail - maybe the file was already deleted
                    Console.WriteLine($"Warning: Could not delete existing avatar: {ex.Message}");
                }
            }

            // Generate unique filename
            var fileExtension = Path.GetExtension(avatar.FileName).ToLower();
            var fileName = $"avatars/user_{id}_{Guid.NewGuid()}{fileExtension}";

            // Upload to S3
            var avatarUrl = await _s3Service.UploadImageToS3(avatar, fileName);

            // Update user record
            var updatedUser = await _userService.UpdateUserAvatarAsync(id, avatarUrl);
            if (updatedUser == null)
                return StatusCode(500, "Failed to update user avatar URL.");

            return Ok(new { avatarUrl, user = updatedUser });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to upload avatar: {ex.Message}");
        }
    }

    [HttpDelete("{id}/avatar")]
    public async Task<ActionResult> DeleteAvatar(int id)
    {
        // Validate user exists
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound($"User with ID {id} not found.");

        if (string.IsNullOrEmpty(user.Avatar))
            return BadRequest("User has no avatar to delete.");

        try
        {
            // Delete from S3
            await _s3Service.DeleteImageFromS3(user.Avatar);

            // Update user record
            var success = await _userService.DeleteUserAvatarAsync(id);
            if (!success)
                return StatusCode(500, "Failed to update user record.");

            return Ok(new { message = "Avatar deleted successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to delete avatar: {ex.Message}");
        }
    }
}