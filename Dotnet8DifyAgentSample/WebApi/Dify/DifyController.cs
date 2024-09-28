using Dotnet8DifyAgentSample.Filters;
using Dotnet8DifyAgentSample.Models;
using Dotnet8DifyAgentSample.Services.DifyWorkflow;
using Dotnet8DifyAgentSample.Services.DifyWorkflow.Dtos;
using Dotnet8DifyAgentSample.WebApi.Dify.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet8DifyAgentSample.WebApi.Dify;

[Route("api/[controller]/[action]")]
[ServiceFilter(typeof(CustomExceptionFilter))]
[ServiceFilter(typeof(CustomValidationFilter))]
[ApiController]
public class DifyController : ControllerBase
{
    private readonly DifyCreateProductService _difyCreateProductService;
    private readonly string _difyUserId;

    public DifyController(DifyCreateProductService difyCreateProductService, IConfiguration configuration)
    {
        _difyCreateProductService = difyCreateProductService;
        _difyUserId = configuration["DifyUserId"];
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductDetail([FromBody] CreateWorkflowRequest request)
    {
        var inputs = new Dictionary<string, object>();
        inputs.Add("product_name", request.ProductName);
        
        var runWorkflowRequest = new DifyWorkflowRequest
        {
            Inputs = inputs,
            ResponseMode = "blocking",
            User = _difyUserId
        };
        
        var response = await _difyCreateProductService.CreateProductDetail(runWorkflowRequest);
        var apiResponse = new ApiResponse
        {
            IsSuccess = true,
            Code = ApiStatusCode.Success,
            Body = response.Data.Outputs
        };
        
        return Ok(apiResponse);
    }
}