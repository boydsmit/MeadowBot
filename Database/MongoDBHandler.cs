using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DnsClient.Protocol;
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

        public async Task Upsert<T>(string table, long id, T record)
        {
            var collection = db.GetCollection<T>(table);
            
            var result = collection.ReplaceOne(
                new BsonDocument("_id", id),
                record,
                new UpdateOptions {IsUpsert = true});
        }

        public T LoadRecordById<T>(string table, long id)
        {
            var collection = db.GetCollection<T>(table);
            try
            {
                var filter = Builders<T>.Filter.Eq("_id", id);
                var foundItem = collection.Find(filter).First();
                return foundItem;

            }
            catch (Exception e)
            {
                return default;
            }
        }
    }
}