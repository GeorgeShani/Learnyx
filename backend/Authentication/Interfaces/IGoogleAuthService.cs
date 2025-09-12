using Google.Apis.Auth;
using learnyx.Models.Entities;

namespace learnyx.Authentication.Interfaces;

public interface IGoogleAuthService
{
    Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string idToken);
    Task<User> AuthenticateGoogleUserAsync(string idToken);
    Task<User> AuthenticateWithCodeAsync(string authorizationCode);
}