using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Dotnet8DifyAgentSample.Services.DifyWorkflow.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet8DifyAgentSample.Services.DifyWorkflow;

public class DifyCreateProductService
{
    private readonly string _difyApiUrl;
    private readonly string _difyCreateProductDetailApiKey;
    private readonly IHttpClientFactory _httpClientFactory;

    public DifyCreateProductService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _difyApiUrl = configuration["DifyWorkFlowApiEndpoint"];
        _difyCreateProductDetailApiKey = configuration["DifyCreateProductDetailApiKey"];
        _httpClientFactory = httpClientFactory;
    }

    public async Task<CreateProductResponse> RunWorkflow([FromBody] CreateProductRequest request)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _difyCreateProductDetailApiKey);

        var endpoint = $"{_difyApiUrl}/workflows/run";
        var jsonContent = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(endpoint, content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var runWorkflowResponse = JsonSerializer.Deserialize<CreateProductResponse>(result);
            return runWorkflowResponse;
        }
        else
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            throw new Exception($"Error running workflow: {errorResponse}");
        }
    }
}