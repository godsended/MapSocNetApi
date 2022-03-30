using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
namespace BumberAPI.Models
{
    public class UserDatabase
    {
        private static IMongoCollection<User> Users = null!;
        private static string CollectionName = "Users";

        public UserDatabase()
        {
            var mongoClient = new MongoClient(DatabaseSettings.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(DatabaseSettings.DatabaseName);

            Users = mongoDatabase.GetCollection<User>(CollectionName);
        }

        public static async Task<List<User>> GetAsync() =>
         await Users.Find(_ => true).ToListAsync();

        public static async Task<User?> GetAsync(string id) =>
            await Users.Find(x => x.Id == id).FirstOrDefaultAsync();

        public static async Task<string?> CreateAsync(User newBook)
        {
            await Users.InsertOneAsync(newBook);
            return newBook.Id;
        }

        public static async Task UpdateAsync(string id, User updatedBook) =>
            await Users.ReplaceOneAsync(x => x.Id == id, updatedBook);

        public static async Task RemoveAsync(string id) =>
            await Users.DeleteOneAsync(x => x.Id == id);

        public static async Task<List<User>> Find(List<string> props, List<string> values)
        {
            List<User> users = new List<User>();
            if (props.Count != values.Count || props.Count == 0 || values.Count == 0)
                return users;

            BsonArray bar = new BsonArray();
            for (int i = 0; i < props.Count; i++)
                bar.Add(new BsonDocument(props[i], values[i]));
            BsonDocument filter = new BsonDocument("$and", bar);
            users = await Users.Find(filter).ToListAsync();

            return users;
        }
    }
}
