using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dotnet8DifyAgentSample.Models.MongoDB.Entities;

public class Conversation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string ConversationId { get; set; }

    [BsonElement("UserId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; }

    [BsonElement("Summarization")]
    public string Summarization { get; set; }

    [BsonElement("CreateAt")]
    public DateTime CreateAt { get; set; }

    [BsonElement("UpdateAt")]
    public DateTime UpdateAt { get; set; }
}