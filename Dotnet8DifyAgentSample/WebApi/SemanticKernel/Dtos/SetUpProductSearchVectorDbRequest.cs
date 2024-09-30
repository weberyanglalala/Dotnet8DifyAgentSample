using System.Text.Json.Serialization;

namespace Dotnet8DifyAgentSample.WebApi.SemanticKernel.Dtos;

public class SetUpProductSearchVectorDbRequest
{
    [JsonPropertyName("start_index")] public int StartIndex { get; set; }
    [JsonPropertyName("count")] public int Count { get; set; }
}