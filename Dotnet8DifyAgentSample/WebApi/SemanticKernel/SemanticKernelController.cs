using Dotnet8DifyAgentSample.Models;
using Dotnet8DifyAgentSample.Services.SemanticKernel;
using Dotnet8DifyAgentSample.WebApi.SemanticKernel.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet8DifyAgentSample.WebApi.SemanticKernel;

[Route("api/[controller]/[action]")]
[ApiController]
public class SemanticKernelController : ControllerBase
{
    private readonly ProductDetailGenerateClient _productDetailGenerateClient;
    private readonly ProductChatService _productChatService;

    public SemanticKernelController(ProductDetailGenerateClient productDetailGenerateClient,
        ProductChatService productChatService)
    {
        _productDetailGenerateClient = productDetailGenerateClient;
        _productChatService = productChatService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductDetail(CreateProductDetailRequest request)
    {
        var response = await _productDetailGenerateClient.CreateProductDetail(request.ProductName);
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
}