using System.Text.Json.Serialization;

namespace Dotnet8DifyAgentSample.WebApi.SemanticKernel.Dtos;

public class GetProductChatResultRequest
{
    [JsonPropertyName("input")]
    public string Input { get; set; }
}