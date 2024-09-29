namespace Dotnet8DifyAgentSample.Models;

#nullable enable
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}