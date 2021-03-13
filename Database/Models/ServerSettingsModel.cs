using System;
using MongoDB.Bson;

namespace BunniBot.Database.Models
{
    public class ServerSettingsModel
    {
        public BsonValue Id;
        public BsonValue Value;
        public BsonValue MetaData;

        public ServerSettingsModel(BsonValue id, object value, object metaData = null)
        {
            Id = id;
            Value = BsonValue.Create(value);
            MetaData = BsonValue.Create(metaData);
        }

        public BsonValue GetId()
        {
            return Id;
        }

        public BsonValue GetValue()
        {
            return Value;
        }

        public BsonValue GetMetaData()
        {
            return MetaData;
        }
    }
    
}