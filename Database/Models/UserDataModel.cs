using System;
using MongoDB.Bson.Serialization.Attributes;

namespace BunniBot.Database.Models
{
    public class UserDataModel
    {
        [BsonId] public long UserId;
        public string UserName;
        public MuteDataModel MuteData;
        public long UserCurrency;
        public long UserXp;
        public int UserLevel;
        public long LastUserCurrencyUpdateTimeBinary;

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

        public void AddUserCurrency(long currency)
        {
            UserCurrency += currency;
        }
        
        public void SubtractUserCurrency(long currency)
        {
            UserCurrency -= currency;
        }

        public long GetUserCurrency()
        {
            return UserCurrency;
        }

        public void AddUserXp(long xp)
        {
            UserXp += xp;
        }

      

        public long GetUserXp()
        {
            return UserXp;
        }

        public void AddUserLevel(int levels)
        {
            UserLevel += levels;
        }

        public int GetUserLevel()
        {
            return UserLevel;
        }
        
        public void SetLastCurrencyUpdateAsBinary(long timeAsBinary)
        {
            LastUserCurrencyUpdateTimeBinary = timeAsBinary;
        }

        public long GetLastCurrencyUpdateAsBinary()
        {
            return LastUserCurrencyUpdateTimeBinary;
        }
    }
}