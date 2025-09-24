using AutoMapper;
using learnyx.Data;
using learnyx.Models.DTOs;
using System.Security.Claims;
using learnyx.Models.Requests;
using learnyx.Models.Responses;
using learnyx.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using learnyx.Authentication.Interfaces;

namespace learnyx.Services.Implementation;

public class ProfileService : IProfileService
{
    private readonly DataContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAmazonS3Service _amazonS3Service;
    private readonly IMapper _mapper;

    public ProfileService(
        DataContext context, 
        IHttpContextAccessor httpContextAccessor, 
        IAmazonS3Service amazonS3Service,
        IMapper mapper
    ) {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _amazonS3Service = amazonS3Service;
        _mapper = mapper;
    }

    public async Task<UserDTO?> GetCurrentUserProfileAsync()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return null;
        
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        var userDto = _mapper.Map<UserDTO>(user);
        return userDto;
    }

    public async Task<UserDTO?> UpdateProfileAsync(UpdateProfileRequest request)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return null;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return null;

        // Update fields
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.Bio = request.Bio;

        await _context.SaveChangesAsync();

        return _mapper.Map<UserDTO>(user);
    }

    
    public async Task<UserDTO?> UpdateProfilePictureAsync(UpdateProfilePictureRequest request)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return null;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null) return null;

        if (request.ProfilePicture is null) 
            return _mapper.Map<UserDTO>(user);

        if (user.Avatar is not null && user.Avatar.Contains("amazonaws"))
            await _amazonS3Service.DeleteImageFromS3(user.Avatar);

        var key = $"users/profile/{user.Id}_{Guid.NewGuid()}{Path.GetExtension(request.ProfilePicture.FileName)}";
        var newAvatarUrl = await _amazonS3Service.UploadImageToS3(request.ProfilePicture, key);

        user.Avatar = newAvatarUrl;

        await _context.SaveChangesAsync();
        return _mapper.Map<UserDTO>(user);
    }

    public async Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
            return new ChangePasswordResult { Success = false, Message = "Invalid user context" };

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return new ChangePasswordResult { Success = false, Message = "User not found" };

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
            return new ChangePasswordResult { Success = false, Message = "Invalid current password" };

        if (request.NewPassword != request.ConfirmNewPassword)
            return new ChangePasswordResult { Success = false, Message = "Passwords do not match" };

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _context.SaveChangesAsync();

        return new ChangePasswordResult { Success = true, Message = "Password changed successfully" };
    }
}