using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace BunniBot.Database.Models
{
    public class UserLogsModel
    {
        [BsonId] public long UserId;

        public string Username;

        public List<ActionsModel> Actions = new List<ActionsModel>();

        public UserLogsModel(long userId, string username, List<ActionsModel> actions)
        {
            UserId = userId;
            Username = username;
            Actions = actions;
        }
    }
}