using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BunniBot.Database
{
    public class MongoDBHandler
    {
        private IMongoDatabase db;

        public MongoDBHandler(string database)
        {
            var client = new MongoClient();
            db = client.GetDatabase(database);
        }

        public async Task Upsert<T>(string table, BsonValue id, T record)
        {
            var collection = db.GetCollection<T>(table);
            
            var result = collection.ReplaceOne(
                new BsonDocument("_id", id),
                record,
                new UpdateOptions {IsUpsert = true});
        }

        public T LoadRecordByField<T>(string table, string fieldName, object fieldValue)
        {
            var collection = db.GetCollection<T>(table);    
            try
            {
                var filter = Builders<T>.Filter.Eq(fieldName, fieldValue);
                var foundItem = collection.Find(filter).First();
                return foundItem;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default;
            }
        }
    }
}