using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dotnet8DifyAgentSample.Models.MongoDB.Entities;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string MessageId { get; set; }

    [BsonElement("ConversationId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ConversationId { get; set; }

    [BsonElement("MessageType")]
    public MessageType MessageType { get; set; }

    [BsonElement("Content")]
    public string Content { get; set; }

    [BsonElement("Timestamp")]
    public DateTime Timestamp { get; set; }
}

public enum MessageType
{
    System,
    User
}