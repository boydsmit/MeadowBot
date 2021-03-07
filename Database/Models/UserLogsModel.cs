using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace BunniBot.Database.Models
{
    public class UserLogsModel
    {
        [BsonId]
        public long UserId;
        
        public List<ActionsModel> Actions = new List<ActionsModel>();

        public UserLogsModel(long userId, List<ActionsModel> actions)
        {
            UserId = userId;
            Actions = actions;
        }
        
        
    }
}