using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Dotnet8DifyAgentSample.Services.SemanticKernel;

public class ProductChatService
{
    private readonly Kernel _kernel;
    private readonly ChatHistory _chatHistory;
    private readonly IChatCompletionService _chatCompletionService;

    public ProductChatService(Kernel kernel)
    {
        _kernel = kernel;
        _chatHistory = new ChatHistory();
        _chatHistory.AddSystemMessage("你是一個用於管理產品數據的 chat bot。你提供了幾個主要功能:\n\n創建新產品 (CreateNewProduct):\n\n輸入: 產品名稱、售價和描述\n功能: 在數據庫中創建一個新的產品記錄\n輸出: 返回創建的產品對象\n\n\n更新現有產品 (UpdateProductById):\n\n輸入: 產品ID、新的名稱、新的售價和新的描述\n功能: 根據提供的ID更新數據庫中的產品信息\n輸出: 返回更新後的產品對象\n\n\n獲取符合條件的產品數量 (GetProductFilteredResultCount):\n\n輸入: 名稱過濾器、最低價格、最高價格\n功能: 計算符合過濾條件的產品數量\n輸出: 返回符合條件的產品數量\n\n\n獲取符合條件的產品列表 (GetProductFilteredResult):\n\n輸入: 名稱過濾器、最低價格、最高價格、頁碼、每頁數量\n功能: 根據過濾條件和分頁信息獲取產品列表\n輸出: 返回符合條件的產品對象列表\n\n\n\n使用這個 Plugin，你可以執行基本的產品管理操作，包括創建、更新產品，以及根據各種條件搜索和過濾產品。它適用於需要產品數據庫管理功能的應用程序。");
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
    }
    
    public async Task<string> GetResponse(string input)
    {
        _chatHistory.AddUserMessage(input);
        var executionSettings = new OpenAIPromptExecutionSettings()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };
        var result = await _chatCompletionService.GetChatMessageContentAsync(_chatHistory, executionSettings, _kernel);
        return result.Content;
    }
}