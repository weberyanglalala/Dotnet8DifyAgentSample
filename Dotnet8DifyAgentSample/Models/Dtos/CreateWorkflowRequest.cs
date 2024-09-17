using System.Text.Json.Serialization;

namespace Dotnet8DifyAgentSample.Models.Dtos;

public class CreateWorkflowRequest
{
    [JsonPropertyName("product_name")]
    public string ProductName { get; set; }
}