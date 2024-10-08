using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Dotnet8DifyAgentSample.Services.SemanticKernel;

public class TravelChatService
{
    private readonly Kernel _kernel;
    private readonly ChatHistory _chatHistory;
    private readonly IChatCompletionService _chatCompletionService;

    public TravelChatService(Kernel kernel)
    {
        _kernel = kernel;
        _chatHistory = new ChatHistory();
        string systemPrompt = """
                              # Role: Taiwan Travel Recommendation Expert
                              ## Profile
                              - Language: 繁體中文
                              - Description: 你是台灣旅遊推薦專家，擅長提供台灣各地旅遊景點、美食和文化活動的建議。
                              ## Skill-1
                              - 熟悉台灣各大旅遊景點及其特色。
                              - 能夠根據使用者的需求提供個性化的旅遊建議。
                              ## Skill-2
                              - 了解台灣的飲食文化，能夠推薦當地美食。
                              - 熟悉各種交通方式，能夠提供最佳路線建議。
                              ## Skill-3
                              - 根據提供的 Function GetTravelRecommendationsByUserInput 取得相關的旅遊資訊。
                              - 若是呼叫 GetTravelRecommendationsByUserInput Function 的回應，請回應參考格式如下
                              - 旅遊景點: {景點名稱}, Id: {景點編號}, 敘述: {景點敘述}, 網址: https://travel-recommendation/product/{景點Id編號}, 金額: {金額}
                              
                              ## 用戶歷史對談紀錄如下
                              """;
        _chatHistory.AddSystemMessage(systemPrompt);
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string> GetChatResponseByHistoryAndInput(string history, string input)
    {
        _chatHistory.AddSystemMessage(history);
        _chatHistory.AddUserMessage(input);
        var executionSettings = new OpenAIPromptExecutionSettings()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };
        var result = await _chatCompletionService.GetChatMessageContentAsync(_chatHistory, executionSettings, _kernel);
        return result.Content;
    }
}