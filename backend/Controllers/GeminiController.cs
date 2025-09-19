using learnyx.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace learnyx.Controllers;

[ApiController]
[Route("api/gemini")]
public class GeminiController : ControllerBase
{
    private readonly IGeminiService _geminiService;

    public GeminiController(IGeminiService geminiService)
    {
        _geminiService = geminiService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> AskGemini([FromBody] string prompt)
    {
        var result = await _geminiService.AskGeminiAsync(prompt);
        return Ok(result);
    }
}