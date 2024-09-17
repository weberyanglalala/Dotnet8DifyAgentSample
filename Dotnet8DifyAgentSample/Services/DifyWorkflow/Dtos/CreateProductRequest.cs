using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Dotnet8DifyAgentSample.Services.DifyWorkflow.Dtos;

public class CreateProductRequest
{
    [JsonPropertyName("inputs")]
    public Dictionary<string, object> Inputs { get; set; } = new Dictionary<string, object>();
    
    [JsonPropertyName("response_mode")]
    [Required]
    public string ResponseMode { get; set; }
    
    [JsonPropertyName("user")]
    [Required]
    public string User { get; set; }
    
    [JsonPropertyName("files")]
    public List<FileDto> Files { get; set; } = new List<FileDto>();
}

public class FileDto
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("transfer_method")]
    public string TransferMethod { get; set; }
    
    [JsonPropertyName("url")]
    public string Url { get; set; }
    
    [JsonPropertyName("upload_file_id")]
    public string UploadFileId { get; set; }
}