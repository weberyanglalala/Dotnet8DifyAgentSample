using Dotnet8DifyAgentSample.Models;
using Dotnet8DifyAgentSample.Services.OpenAI;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet8DifyAgentSample.WebApi;

[Route("api/[controller]/[action]")]
public class TravelFilterController : ControllerBase
{
    private readonly OpenAIService _openAIService;
    private readonly ILogger<TravelFilterController> _logger;

    public TravelFilterController(OpenAIService openAiService, ILogger<TravelFilterController> logger)
    {
        _openAIService = openAiService;
        _logger = logger;
    }
    
    public async Task<IActionResult> GetFilterResult([FromQuery] string prompt)
    {
        try
        {
            var response = await _openAIService.GetFilterResultAsync(prompt);
            var apiResponse = new ApiResponse
            {
                IsSuccess = true,
                Code = ApiStatusCode.Success,
                Body = response
            };
            return Ok(apiResponse);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while processing the open ai api request.");
            var apiResponse = new ApiResponse
            {
                IsSuccess = false,
                Code = ApiStatusCode.Error,
                Body = "An error occurred while processing open ai api request."
            };
            return Ok(apiResponse);
        }
    }
}