using System;
using Newtonsoft.Json;

namespace BunniBot.Database.Models
{
    public class ShopRoleModel
    {
        public long RoleId;
        public string RoleName;
        public long RoleCost;
        public int RequiredLevel;
        
        public ShopRoleModel(ulong roleId, string roleName, long roleCost, int requiredLevel = 0)
        {
            RoleId = Convert.ToInt64(roleId);
            RoleName = roleName;
            RoleCost = roleCost;
            RequiredLevel = requiredLevel;
        }

        public ulong GetRoleId()
        {
            return Convert.ToUInt64(RoleId);
        }

        public string GetRoleName()
        {
            return RoleName;
        }

        public long GetRoleCost()
        {
            return RoleCost;
        }

        public int GetRequiredLevel()
        {
            return RequiredLevel;
        }
    }
}