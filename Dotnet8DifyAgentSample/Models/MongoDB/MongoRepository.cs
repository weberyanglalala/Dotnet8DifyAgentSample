using Dotnet8DifyAgentSample.Models.MongoDB.Entities;
using Dotnet8DifyAgentSample.Settings;
using MongoDB.Driver;

namespace Dotnet8DifyAgentSample.Models.MongoDB;

public class MongoRepository
{
    private readonly IMongoCollection<User> _users;
    private readonly IMongoCollection<Conversation> _conversations;
    private readonly IMongoCollection<Message> _messages;
    private readonly IMongoClient _mongoClient;
    private readonly MongoDbSettings _settings;

    public MongoRepository(IMongoClient mongoClient, MongoDbSettings settings)
    {
        _mongoClient = mongoClient;
        _settings = settings;
        var database = _mongoClient.GetDatabase(_settings.LineMessageDatabaseName);
        _users = database.GetCollection<User>("Users");
        _conversations = database.GetCollection<Conversation>("Conversations");
        _messages = database.GetCollection<Message>("Messages");
    }

    public async Task CreateUserAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task<User> GetUserAsync(string userId)
    {
        var filter = Builders<User>.Filter.Eq(u => u.UserId, userId);
        return await _users.Find(filter).FirstOrDefaultAsync();
    }

    public async Task CreateConversationByUserIdAsync(string userId, Conversation conversation)
    {
        conversation.UserId = userId;
        await _conversations.InsertOneAsync(conversation);
    }

    public async Task<List<Conversation>> GetConversationsByUserIdAsync(string userId)
    {
        var filter = Builders<Conversation>.Filter.Eq(c => c.UserId, userId);
        return await _conversations.Find(filter).ToListAsync();
    }

    public async Task CreateMessageByConversationIdAsync(string conversationId, Message message)
    {
        message.ConversationId = conversationId;
        message.Timestamp = DateTime.UtcNow;
        await _messages.InsertOneAsync(message);
    }

    public async Task<List<Message>> GetMessagesByConversationIdAsync(string conversationId)
    {
        var filter = Builders<Message>.Filter.Eq(m => m.ConversationId, conversationId);
        return await _messages.Find(filter).ToListAsync();
    }

    public async Task CreateConversationWithMessageAsync(string userId, Conversation conversation, Message message)
    {
        using var session = await _mongoClient.StartSessionAsync();
        session.StartTransaction();

        try
        {
            conversation.UserId = userId;
            await _conversations.InsertOneAsync(session, conversation);

            message.ConversationId = conversation.ConversationId;
            message.Timestamp = DateTime.UtcNow;
            await _messages.InsertOneAsync(session, message);

            await session.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}