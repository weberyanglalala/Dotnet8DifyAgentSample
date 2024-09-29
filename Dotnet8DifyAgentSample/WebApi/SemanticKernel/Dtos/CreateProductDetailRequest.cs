using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dotnet8DifyAgentSample.WebApi.SemanticKernel.Dtos;

public class CreateProductDetailRequest
{
    [Length(5, 30)]
    [Required]
    [JsonPropertyName("product_name")]
    public string ProductName { get; set; }
}