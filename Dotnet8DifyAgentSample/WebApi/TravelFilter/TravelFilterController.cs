using Dotnet8DifyAgentSample.Filters;
using Dotnet8DifyAgentSample.Models;
using Dotnet8DifyAgentSample.Services.OpenAI;
using Dotnet8DifyAgentSample.WebApi.TravelFilter.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet8DifyAgentSample.WebApi.TravelFilter;

[Route("api/[controller]/[action]")]
[ServiceFilter(typeof(CustomExceptionFilter))]
[ServiceFilter(typeof(CustomValidationFilter))]
[ApiController]
public class TravelFilterController : ControllerBase
{
    private readonly OpenAIService _openAIService;

    public TravelFilterController(OpenAIService openAiService)
    {
        _openAIService = openAiService;
    }

    public async Task<IActionResult> GetFilterResult([FromQuery] GetFilterResultRequest request)
    {
        var response = await _openAIService.GetFilterResultAsync(request.Prompt);
        var apiResponse = new ApiResponse
        {
            IsSuccess = true,
            Code = ApiStatusCode.Success,
            Body = response
        };
        return Ok(apiResponse);
    }
}