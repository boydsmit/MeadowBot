using System.Collections.Generic;
using BunniBot.Database.Models;

namespace BunniBot.Services
{
    public class ServerDataManager
    {
        private ulong _id;
    
        private Dictionary<ulong, UserDataModel> _userDataCache;
        private Dictionary<ulong, ShopRoleModel> _shopRoleCache;
        private Dictionary<string, ServerSettingsModel> _serverSettingsCache;

        public ServerDataManager(ulong discordServerId)
        {
            _userDataCache = new Dictionary<ulong, UserDataModel>();
            _shopRoleCache = new Dictionary<ulong, ShopRoleModel>();
            _serverSettingsCache = new Dictionary<string, ServerSettingsModel>();
            _id = discordServerId;
        }
        
        public void SetUserData(ulong userId, UserDataModel userDataModel)
        {
            _userDataCache[userId] = userDataModel;
        }

        public Dictionary<ulong, UserDataModel> GetUserDataModel()
        {
            return _userDataCache;
        }

        public void AddShopRole(ulong roleId, ShopRoleModel shopRoleModel)
        {
            _shopRoleCache.Add(roleId, shopRoleModel);
        }

        public Dictionary<ulong, ShopRoleModel> GetShopRoleModel()
        {
            return _shopRoleCache;
        }

        public void AddServerSettings(string setting, ServerSettingsModel serverSettingsModel) 
        {
            _serverSettingsCache.Add(setting, serverSettingsModel);
        }

        public Dictionary<string, ServerSettingsModel> GetServerSettingsModel()
        {
            return _serverSettingsCache;
        }
    }
}