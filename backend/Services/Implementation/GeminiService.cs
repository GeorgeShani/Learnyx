using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using learnyx.Services.Interfaces;

namespace learnyx.Services.Implementation;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GeminiService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["Gemini:ApiKey"] 
                  ?? throw new Exception("Missing Gemini API key in configuration");
    }

    public async Task<string> AskGeminiAsync(string prompt)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

        const string systemInstruction = "You are the Learnyx AI Assistant. " +
                                         "Learnyx provides courses with a variety of learning materials. " +
                                         "Your role is to guide students: explain concepts, give hints for homework without solving it, " +
                                         "suggest helpful resources, and help them build a personalized learning path.";
        
        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = systemInstruction },
                        new { text = $"Student request: {prompt}" }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Gemini API error: {responseString}");

        // Extract text from response
        var jsonDoc = JsonNode.Parse(responseString);
        var text = jsonDoc?["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

        return text ?? string.Empty;
    }
}