using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Dotnet8DifyAgentSample.Services.SemanticProductSearch;
using Dotnet8DifyAgentSample.Services.SemanticProductSearch.Dtos;
using Microsoft.SemanticKernel;

namespace Dotnet8DifyAgentSample.Services.SemanticKernel;

[Experimental("SKEXP0020")]
public class TravelChatServicePlugin
{
    private readonly SemanticProductSearchService _semanticProductSearchService;

    public TravelChatServicePlugin(SemanticProductSearchService semanticProductSearchService)
    {
        _semanticProductSearchService = semanticProductSearchService;
    }
    
    [KernelFunction("GetTravelRecommendationsByUserInput")]
    [Description("Get travel recommendations by user input")]
    public async Task<List<ProductSearchResult>> GetTravelRecommendationsByUserInput(
        [Description("The user input")] string userInput)
    {
        return await _semanticProductSearchService.GetRecommendationsAsync(userInput);
    }   
}