using System.Text.Json.Serialization;

namespace Dotnet8DifyAgentSample.WebApi.SemanticKernel.Dtos;

public class CreateProductDetailResponse
{
    [JsonPropertyName("product_description")]
    public string ProductDescription { get; set; }
}