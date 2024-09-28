using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dotnet8DifyAgentSample.WebApi.TravelFilter.Dtos;

public class GetFilterResultRequest
{
    [Length(5, 30)]
    [JsonPropertyName("prompt")]
    [Required]
    public string Prompt { get; set; }
}