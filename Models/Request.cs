using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BumberAPI.Models
{
    public class Request
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? SenderId { get; set; }
        public string? RecieverId { get; set; }
        public string? Info { get; set; }
    }
}

