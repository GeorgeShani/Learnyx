using System.Security.Cryptography;

namespace learnyx.Utilities.Helpers;

public static class VerificationCodeGenerator
{
    public static string Generate6DigitCode()
    {
        // Generates a number between 0 and 999999, and pads with zeros if needed
        return RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");
    }
}