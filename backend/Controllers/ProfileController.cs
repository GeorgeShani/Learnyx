using learnyx.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using learnyx.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace learnyx.Controllers;

[ApiController]
[Route("api/users/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(IProfileService profileService, ILogger<ProfileController> logger)
    {
        _profileService = profileService;
        _logger = logger;
    }

    // GET api/users/profile
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var profile = await _profileService.GetCurrentUserProfileAsync();
            if (profile is null) return NotFound();

            return Ok(profile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving profile");
            return StatusCode(500, "An error occurred while retrieving the profile.");
        }
    }

    // PUT api/users/profile
    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var updatedProfile = await _profileService.UpdateProfileAsync(request);
            if (updatedProfile is null) return NotFound();

            return Ok(updatedProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile");
            return StatusCode(500, "An error occurred while updating the profile.");
        }
    }

    // PUT api/users/profile/password
    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var result = await _profileService.ChangePasswordAsync(request);
            if (!result.Success) return BadRequest(result.Message);

            return Ok(result.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return StatusCode(500, "An error occurred while changing the password.");
        }
    }

    // PUT api/users/profile/picture
    [HttpPut("picture")]
    public async Task<IActionResult> UpdateProfilePicture([FromForm] UpdateProfilePictureRequest request)
    {
        try
        {
            var updatedProfile = await _profileService.UpdateProfilePictureAsync(request);
            if (updatedProfile is null) return NotFound();

            return Ok(updatedProfile);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile picture");
            return StatusCode(500, "An error occurred while updating the profile picture.");
        }
    }
}