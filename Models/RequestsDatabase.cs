using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
namespace BumberAPI.Models
{
    public class RequestsDatabase
    {
        private static IMongoCollection<Request>? requests = null!;
        private static IMongoCollection<Request>? Requests
        {
            get
            {
                if (requests == null)
                    new RequestsDatabase();
                return requests;
            }
            set
            {
                requests = value;
            }
        }
        private static string CollectionName = "Requests";

        public RequestsDatabase()
        {
            var mongoClient = new MongoClient(DatabaseSettings.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(DatabaseSettings.DatabaseName);

            Requests = mongoDatabase.GetCollection<Request>(CollectionName);
        }

        public static async Task<List<Request>> GetAsync() =>
         await Requests.Find(_ => true).ToListAsync();

        public static async Task<Request?> GetAsync(string id) =>
            await Requests.Find(x => x.Id == id).FirstOrDefaultAsync();

        public static async Task<string?> CreateAsync(Request newBook)
        {
            await Requests.InsertOneAsync(newBook);
            return newBook.Id;
        }

        public static async Task UpdateAsync(string id, Request updatedBook) =>
            await Requests.ReplaceOneAsync(x => x.Id == id, updatedBook);

        public static async Task RemoveAsync(string id) =>
            await Requests.DeleteOneAsync(x => x.Id == id);

        public static async Task<List<Request>> Find(List<string> props, List<string> values)
        {
            List<Request> images = new List<Request>();
            if (props.Count != values.Count || props.Count == 0 || values.Count == 0)
                return images;

            if (Requests == null)
                return new List<Request>();
     
            BsonArray bar = new BsonArray();
            for (int i = 0; i < props.Count; i++)
                bar.Add(new BsonDocument(props[i], values[i]));
            BsonDocument filter = new BsonDocument("$and", bar);
            images = await Requests.Find(filter).ToListAsync();

            return images;
        }
    }
}
