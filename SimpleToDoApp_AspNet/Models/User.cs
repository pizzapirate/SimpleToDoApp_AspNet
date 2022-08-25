using MongoDB.Bson.Serialization.Attributes;

namespace SimpleToDoApp_AspNet.Models
{
    public class User
    {
        public string? FirstName { get; set; }
        [BsonId]
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
