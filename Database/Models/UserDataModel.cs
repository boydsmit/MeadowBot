using System;
using MongoDB.Bson.Serialization.Attributes;

namespace BunniBot.Database.Models
{
    public class UserDataModel
    {
        [BsonId] public long UserId;
        public string UserName;
        public MuteDataModel MuteData;

        public UserDataModel(ulong userId, string userName)
        {
            UserId = Convert.ToInt64(userId);
            UserName = userName;
        }

        public ulong GetUserId()
        {
            return Convert.ToUInt64(UserId);
        }

        public string GetUserName()
        {
            return UserName;
        }

        public MuteDataModel GetMuteData()
        {
            return MuteData;
        }

        public void SetMuteData(MuteDataModel muteData)
        {
            MuteData = muteData;
        }
    }
}