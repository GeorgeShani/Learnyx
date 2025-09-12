using learnyx.Models.Auth;
using learnyx.Models.Entities;

namespace learnyx.Authentication.Interfaces;

public interface IFacebookAuthService
{
    Task<bool> ValidateAccessTokenAsync(string accessToken);
    Task<FacebookUserInfo> GetUserInfoAsync(string accessToken);
    Task<User> AuthenticateAsync(string accessToken);
    Task<User> AuthenticateWithCodeAsync(string authorizationCode);   
}