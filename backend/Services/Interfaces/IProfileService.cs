using learnyx.Models.DTOs;
using learnyx.Models.Requests;
using learnyx.Models.Responses;

namespace learnyx.Services.Interfaces;

public interface IProfileService
{
    Task<UserDTO?> GetCurrentUserProfileAsync();
    Task<UserDTO?> UpdateProfileAsync(UpdateProfileRequest request);
    Task<UserDTO?> UpdateProfilePictureAsync(UpdateProfilePictureRequest request);
    Task<ChangePasswordResult> ChangePasswordAsync(ChangePasswordRequest request);
}