namespace learnyx.Services.Interfaces;

public interface IGeminiService
{
    Task<string> AskGeminiAsync(string prompt);
}