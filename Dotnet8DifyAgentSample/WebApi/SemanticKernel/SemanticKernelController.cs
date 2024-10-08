using System.Diagnostics.CodeAnalysis;
using Dotnet8DifyAgentSample.Models;
using Dotnet8DifyAgentSample.Services.SemanticKernel;
using Dotnet8DifyAgentSample.Services.SemanticProductSearch;
using Dotnet8DifyAgentSample.WebApi.SemanticKernel.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet8DifyAgentSample.WebApi.SemanticKernel;

[Route("api/[controller]/[action]")]
[ApiController]
[Experimental("SKEXP0020")]
public class SemanticKernelController : ControllerBase
{
    private readonly ProductDetailGenerateService _productDetailGenerateService;
    private readonly ProductChatService _productChatService;
    private readonly SemanticProductSearchService _semanticProductSearchService;

    public SemanticKernelController(ProductDetailGenerateService productDetailGenerateService,
        ProductChatService productChatService, SemanticProductSearchService semanticProductSearchService)
    {
        _productDetailGenerateService = productDetailGenerateService;
        _productChatService = productChatService;
        _semanticProductSearchService = semanticProductSearchService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductDetail(CreateProductDetailRequest request)
    {
        var response = await _productDetailGenerateService.CreateProductDetail(request.ProductName);
        var apiResponse = new ApiResponse()
        {
            IsSuccess = true,
            Code = ApiStatusCode.Success,
            Body = new CreateProductDetailResponse()
            {
                ProductDescription = response
            }
        };
        return Ok(apiResponse);
    }

    [HttpPost]
    public async Task<IActionResult> GetProductChatResult([FromBody] GetProductChatResultRequest request)
    {
        var response = await _productChatService.GetResponse(request.Input);
        var apiResponse = new ApiResponse()
        {
            IsSuccess = true,
            Code = ApiStatusCode.Success,
            Body = response
        };
        return Ok(apiResponse);
    }
    
    [HttpPost]
    public async Task<IActionResult> SetUpProductSearchVectorDb([FromBody] SetUpProductSearchVectorDbRequest request)
    {
        await _semanticProductSearchService.FetchAndSaveProductDocumentsAsync(request.StartIndex, request.Count);
        return Ok();
    }
    
    public async Task<IActionResult> GetRecommendationsAsync([FromQuery]string userInput)
    {
        var result = await _semanticProductSearchService.GetRecommendationsAsync(userInput);
        var apiResponse = new ApiResponse()
        {
            IsSuccess = true,
            Code = ApiStatusCode.Success,
            Body = result
        };
        return Ok(apiResponse);
    }
}