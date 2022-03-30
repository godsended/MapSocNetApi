using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace BumberAPI.Models
{
    public class User
    {
        #region Auth
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? Mail { get; set; }
        public string? PassHash { get; set; }
        public string? ActionToken { get; set; }
        public string? ConfirmToken { get; set; }
        #endregion
        #region Location
        public double LocationX { get; set; }
        public double LocationY { get; set; }
        #endregion
        #region Info
        public string? InviteCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? OnlineStatus { get; set; }
        public string? Gender { get; set; }
        public int Status { get; set; }
        public string Description { get; set; } = "";
        public string ShortDescription { get; set; } = "";
        public long Rate { get; set; }
        #endregion
    }
}
