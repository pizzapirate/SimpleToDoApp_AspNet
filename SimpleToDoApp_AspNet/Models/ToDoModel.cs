using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class ToDoModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string Id { get; set; }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string ToDoTask { get; set; }
}
