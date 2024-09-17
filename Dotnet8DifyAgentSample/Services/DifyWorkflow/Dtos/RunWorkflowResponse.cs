using System.Text.Json.Serialization;

namespace Dotnet8DifyAgentSample.Services.DifyWorkflow.Dtos;

public class RunWorkflowResponse
{
    [JsonPropertyName("workflow_run_id")]
    public string WorkflowRunId { get; set; }

    [JsonPropertyName("task_id")]
    public string TaskId { get; set; }

    [JsonPropertyName("data")]
    public WorkflowData Data { get; set; }
}

public class WorkflowData
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("workflow_id")]
    public string WorkflowId { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("outputs")]
    public object Outputs { get; set; }

    [JsonPropertyName("error")]
    public string Error { get; set; }

    [JsonPropertyName("elapsed_time")]
    public float ElapsedTime { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }

    [JsonPropertyName("total_steps")]
    public int TotalSteps { get; set; }

    [JsonPropertyName("created_at")]
    public long CreatedAt { get; set; }

    [JsonPropertyName("finished_at")]
    public long FinishedAt { get; set; }
}