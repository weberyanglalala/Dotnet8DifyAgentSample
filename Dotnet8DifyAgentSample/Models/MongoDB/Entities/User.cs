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
}