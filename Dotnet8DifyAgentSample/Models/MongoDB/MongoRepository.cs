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

    public async Task<User> GetUserByLineUserIdAsync(string lineUserId)
    {
        var filter = Builders<User>.Filter.Eq(u => u.LineUserId, lineUserId);
        return await _users.Find(filter).FirstOrDefaultAsync();
    }
    
    
    public async Task<User> GetUserAsync(string userId)
    {
        var filter = Builders<User>.Filter.Eq(u => u.UserId, userId);
        return await _users.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Conversation> CreateConversationByUserIdAsync(string userId, Conversation conversation)
    {
        conversation.UserId = userId;
        await _conversations.InsertOneAsync(conversation);
        return conversation;
    }
    
    public async Task<Conversation> GetConversationAsync(string conversationId)
    {
        var filter = Builders<Conversation>.Filter.Eq(c => c.ConversationId, conversationId);
        return await _conversations.Find(filter).FirstOrDefaultAsync();
    }
    
    public async Task<Conversation> UpdateConversationAsync(Conversation conversation)
    {
        var filter = Builders<Conversation>.Filter.Eq(c => c.ConversationId, conversation.ConversationId);
        await _conversations.ReplaceOneAsync(filter, conversation);
        return conversation;
    }

    public async Task<List<Conversation>> GetConversationsByUserIdAsync(string userId)
    {
        var filter = Builders<Conversation>.Filter.Eq(c => c.UserId, userId);
        var sort = Builders<Conversation>.Sort.Descending(c => c.CreateAt);
        return await _conversations.Find(filter).Sort(sort).ToListAsync();
    }
    
    public async Task<Conversation> GetLatestConversationByUserIdAsync(string userId)
    {
        var filter = Builders<Conversation>.Filter.Eq(c => c.UserId, userId);
        var sort = Builders<Conversation>.Sort.Descending(c => c.CreateAt);
        return await _conversations.Find(filter).Sort(sort).FirstOrDefaultAsync();
    }

    public async Task<Message> CreateMessageByConversationIdAsync(string conversationId, Message message)
    {
        message.ConversationId = conversationId;
        message.Timestamp = DateTime.UtcNow;
        await _messages.InsertOneAsync(message);
        return message;
    }

    public async Task<List<Message>> GetMessagesByConversationIdAsync(string conversationId)
    {
        var filter = Builders<Message>.Filter.Eq(m => m.ConversationId, conversationId);
        var sort = Builders<Message>.Sort.Ascending(m => m.Timestamp);
        return await _messages.Find(filter).Sort(sort).ToListAsync();
    }

    public async Task<Conversation> CreateConversationWithMessageAsync(string userId, Conversation conversation, Message message)
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
            return conversation;
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}