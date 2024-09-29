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

    public SemanticKernelController(ProductDetailGenerateClient productDetailGenerateClient)
    {
        _productDetailGenerateClient = productDetailGenerateClient;
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
}