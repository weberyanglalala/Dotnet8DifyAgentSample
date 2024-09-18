using System.Text.Json;
using Dotnet8DifyAgentSample.Services.OpenAI.Dtos;
using OpenAI.Chat;

namespace Dotnet8DifyAgentSample.Services.OpenAI;

public class OpenAIService
{
    private readonly string _apiKey;

    public OpenAIService(IConfiguration configuration)
    {
        _apiKey = configuration["OpenAIApiKey"];
    }

    public async Task<FilterResult> GetFilterResultAsync(string prompt)
    {
        ChatClient client = new ChatClient("gpt-4o-mini", _apiKey);
        
        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                name: "travel_assistant_prompts",
                jsonSchema: BinaryData.FromString("""
                                                  {
                                                    "type": "object",
                                                    "properties": {
                                                      "category_name": {
                                                        "type": "string",
                                                        "enum":["文化體驗","博物館參觀","歷史古蹟","藝術展覽","傳統表演","當地節慶","自然探索","國家公園","海灘度假","山岳hiking","生態旅遊","觀星活動","美食之旅","當地特色餐廳","美食街巡禮","烹飪課程","葡萄酒品鑒","農場採摘","冒險活動","極限運動","水上活動","叢林探險","沙漠safari","滑雪度假","休閒放鬆","溫泉SPA","瑜伽靜修","海濱度假村","城市漫步","購物之旅"]
                                                      },
                                                      "max_price": {
                                                        "type": "number",
                                                        "description": "The maximum price of the travel that evaluated."
                                                      },
                                                      "min_price": {
                                                        "type": "number",
                                                        "description": "The minimum price of the travel that evaluated."
                                                      },
                                                      "district": {
                                                        "type": "string",
                                                        "description": "The district of the travel package that must be in Taiwan."
                                                      }
                                                    },
                                                    "required": ["category_name", "max_price", "min_price", "district"],
                                                    "additionalProperties": false
                                                  }
                                                  """),
                strictSchemaEnabled: true)
        };
        
        List<ChatMessage> messages =
        [
            new SystemChatMessage(
                "您是專業的旅遊推薦專家，能夠根據用戶的需求和喜好提供量身定制的旅遊計劃和建議。從給定文本中提取類別名稱、您評估的最高價格、您評估的最低價格和區域信息"),
            new UserChatMessage(prompt),
        ];

        ChatCompletion chatCompletion = await client.CompleteChatAsync(messages, options);
        var chatCompletionText = chatCompletion.Content[0].Text;
        var result = JsonSerializer.Deserialize<FilterResult>(chatCompletionText);
        return result;
    }
}