using System.Security.Cryptography;

namespace learnyx.Utilities.Helpers;

public static class Verification
{
    public static string Generate6DigitCode()
    {
        // Generates a number between 0 and 999999, and pads with zeros if needed
        return RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");
    }

    public static string GenerateResetToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32); // generates 32 random bytes
        return Convert.ToBase64String(bytes);
    }
}