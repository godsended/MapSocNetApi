using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BumberAPI.Models
{
    public class Image
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Path { get; set; }
        public string? UserId { get; set; }
        public bool? IsAvatar { get; set; }
        public int? LikesCount { get; set; }
    }
}

