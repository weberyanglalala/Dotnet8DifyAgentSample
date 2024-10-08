using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dotnet8DifyAgentSample.Models.MongoDB.Entities;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; }
    
    [BsonElement("LineUserId")]
    public string LineUserId { get; set; }
    
    [BsonElement("Email")]
    public string Email { get; set; }
    
    [BsonElement("CreateAt")]
    public DateTime CreateAt { get; set; }
    
    [BsonElement("UpdateAt")]
    public DateTime UpdateAt { get; set; }
}