using Dotnet8DifyAgentSample.Models;
using Dotnet8DifyAgentSample.Services.DifyWorkflow;
using Dotnet8DifyAgentSample.Services.DifyWorkflow.Dtos;
using Dotnet8DifyAgentSample.WebApi.Dify.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet8DifyAgentSample.WebApi.Dify;

[Route("api/[controller]/[action]")]
[ApiController]
public class DifyController : ControllerBase
{
    private readonly DifyCreateProductService _difyCreateProductService;
    private readonly string _difyUserId;
    private readonly ILogger<DifyController> _logger;

    public DifyController(DifyCreateProductService difyCreateProductService, IConfiguration configuration,
        ILogger<DifyController> logger)
    {
        _difyCreateProductService = difyCreateProductService;
        _difyUserId = configuration["DifyUserId"];
        _logger = logger;
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
        try
        {
            var response = await _difyCreateProductService.CreateProductDetail(runWorkflowRequest);
            var apiResponse = new ApiResponse
            {
                IsSuccess = true,
                Code = ApiStatusCode.Success,
                Body = response.Data.Outputs
            };
            return Ok(apiResponse);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while creating the workflow.");
            var apiResponse = new ApiResponse
            {
                IsSuccess = false,
                Code = ApiStatusCode.Error,
                Body = "An error occurred while processing your request."
            };
            return Ok(apiResponse);
        }
    }
}