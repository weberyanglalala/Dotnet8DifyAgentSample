using Dotnet8DifyAgentSample.Models.MongoDB;
using Dotnet8DifyAgentSample.Models.MongoDB.Entities;
using Dotnet8DifyAgentSample.Services.SemanticKernel;

namespace Dotnet8DifyAgentSample.Services.LineMessage;

public class LineMessageService
{
    private readonly MongoRepository _repository;
    private const int MaxMessagesPerConversation = 10; // 設定對話紀錄上限
    private readonly ChatSummarizationService _chatSummarizationService;
    private readonly TravelChatService _travelChatService;

    public LineMessageService(MongoRepository repository, ChatSummarizationService chatSummarizationService,
        TravelChatService travelChatService)
    {
        _repository = repository;
        _chatSummarizationService = chatSummarizationService;
        _travelChatService = travelChatService;
    }

    public async Task<string> ProcessMessageAsync(string lineUserId, string messageContent)
    {
        // 1. 確認使用者是否存在，若不存在新增一個
        var user = await CreateUserIfNotExisted(lineUserId);
        // 2. 取得使用者 id，根據使用者 id 搜尋是否有存在的對話紀錄
        var conversation = await GetOrCreateConversation(user.UserId);
        var summarization = conversation.Summarization ?? "目前沒有相關對話紀錄";
        // 3. 將對話紀錄新增一筆訊息
        await CreateMessageByConversationIdAsync(conversation.ConversationId, messageContent, MessageType.User);
        // 4. 根據對話紀錄的摘要和使用者輸入取得回應
        var chatResponse = await _travelChatService.GetChatResponseByHistoryAndInput(summarization, messageContent);
        // 5. 將回應新增一筆訊息
        await CreateMessageByConversationIdAsync(conversation.ConversationId, chatResponse, MessageType.System);
        // 6. 更新對話紀錄的摘要
        await UpdateSummarization(conversation.ConversationId);
        // 7. 回傳訊息
        return chatResponse;
    }

    private async Task<Conversation> UpdateSummarization(string conversationId)
    {
        var conversation = await _repository.GetConversationAsync(conversationId);
        if (conversation.Summarization is null)
        {
            conversation.Summarization = "目前沒有相關對話紀錄";
        }
        else
        {
            var latestMessages = await _repository.GetMessagesByConversationIdAsync(conversationId);
            var chatHistory = latestMessages.Select(m => $"{m.MessageType.ToString()} > {m.Content}").ToList();
            var stringChatHistory = string.Join("\n", chatHistory);
            var newSummarization = await _chatSummarizationService.GetSummarization(stringChatHistory);
            conversation.Summarization = newSummarization;
        }

        await _repository.UpdateConversationAsync(conversation);
        return conversation;
    }

    private async Task<User> CreateUserIfNotExisted(string lineUserId)
    {
        var user = await _repository.GetUserByLineUserIdAsync(lineUserId);
        if (user == null)
        {
            user = new User { LineUserId = lineUserId, CreateAt = DateTime.Now };
            await _repository.CreateUserAsync(user);
        }

        return user;
    }

    private async Task<Conversation> GetOrCreateConversation(string userId)
    {
        var latestConversation = await _repository.GetLatestConversationByUserIdAsync(userId);

        if (latestConversation == null || await IsConversationFull(latestConversation.ConversationId))
        {
            // 如果沒有對話紀錄或最新的對話已滿，創建新的對話
            return await CreateConversationByUserId(userId);
        }

        return latestConversation;
    }

    private async Task<bool> IsConversationFull(string conversationId)
    {
        var messages = await _repository.GetMessagesByConversationIdAsync(conversationId);
        return messages.Count >= MaxMessagesPerConversation;
    }

    private async Task<Conversation> CreateConversationByUserId(string userId)
    {
        var conversation = new Conversation { UserId = userId, CreateAt = DateTime.Now };
        await _repository.CreateConversationByUserIdAsync(userId, conversation);
        return conversation;
    }

    private async Task<Message> CreateMessageByConversationIdAsync(string conversationId, string messageContent,
        MessageType messageType)
    {
        var message = new Message
        {
            MessageType = messageType,
            Content = messageContent,
            Timestamp = DateTime.UtcNow
        };
        var result = await _repository.CreateMessageByConversationIdAsync(conversationId, message);
        return result;
    }
}