using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
namespace BumberAPI.Models
{
    public class ImagesDatabase
    {
        private static IMongoCollection<Image>? images = null!;
        private static IMongoCollection<Image>? Images
        {
            get
            {
                if (images == null)
                    new ImagesDatabase();
                return images;
            }
            set
            {
                images = value;
            }
        }
        private static string CollectionName = "Images";

        public ImagesDatabase()
        {
            var mongoClient = new MongoClient(DatabaseSettings.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(DatabaseSettings.DatabaseName);

            Images = mongoDatabase.GetCollection<Image>(CollectionName);
        }

        public static async Task<List<Image>> GetAsync() =>
         await Images.Find(_ => true).ToListAsync();

        public static async Task<Image?> GetAsync(string id) =>
            await Images.Find(x => x.Id == id).FirstOrDefaultAsync();

        public static async Task<string?> CreateAsync(Image newBook)
        {
            await Images.InsertOneAsync(newBook);
            return newBook.Id;
        }

        public static async Task UpdateAsync(string id, Image updatedBook) =>
            await Images.ReplaceOneAsync(x => x.Id == id, updatedBook);

        public static async Task RemoveAsync(string id) =>
            await Images.DeleteOneAsync(x => x.Id == id);

        public static async Task<List<Image>> Find(List<string> props, List<string> values, List<string>? boolProps = null, List<bool>? boolValues = null)
        {
            List<Image> images = new List<Image>();
            if (props == null || values == null)
                return images;

            if (props.Count != values.Count || props.Count == 0 || values.Count == 0)
                return images;

            if (Images == null)
                return new List<Image>();
     
            BsonArray bar = new BsonArray();
            for (int i = 0; i < props.Count; i++)
                bar.Add(new BsonDocument(props[i], values[i]));

            if(boolProps!=null && boolValues!=null)
            {
                if(boolProps.Count == boolValues.Count)
                {
                    for (int i = 0; i < boolProps.Count; i++)
                        bar.Add(new BsonDocument(boolProps[i], boolValues[i]));
                }
            }
            BsonDocument filter = new BsonDocument("$and", bar);
            images = await Images.Find(filter).ToListAsync();

            return images;
        }
    }
}
