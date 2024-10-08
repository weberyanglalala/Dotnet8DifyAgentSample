using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dotnet8DifyAgentSample.WebApi.Dify.Dtos;

public class CreateWorkflowRequest
{
    [JsonPropertyName("product_name")]
    [Required]
    [Length(5, 30)]
    public string ProductName { get; set; }
}