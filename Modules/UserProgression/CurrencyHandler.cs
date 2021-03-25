using System;
using System.Collections.Generic;
using BunniBot.Database.Models;
using BunniBot.Services;
using Discord.Commands;

namespace BunniBot.Modules.UserProgression
{
    public class CurrencyHandler : ModuleBase<SocketCommandContext>
    {
        private SocketCommandContext _context;
        private Dictionary<ulong,  ServerDataManager> _serverDataCache;
        
        public void Initialize(SocketCommandContext context, ref Dictionary<ulong, ServerDataManager> serverData)
        {
             _context = context;
             _serverDataCache = serverData;
        }
        
        public void MessageRecieved()
        {
            var serverData = _serverDataCache[_context.Guild.Id];
            UserDataModel currentUserData;

            try
            { 
                currentUserData = serverData.GetUserDataModel()[_context.User.Id];
            }
            catch(Exception)
            {
                currentUserData = new UserDataModel(_context.User.Id, _context.User.Username);
                currentUserData.LastUserCurrencyUpdateTimeBinary = 0;
                serverData.SetUserData(_context.User.Id, currentUserData);
            }


            var random = new Random();
            var randomNumber = random.Next(25, 60);
            
            var fiveMinutesAsBinary = new DateTime().AddMinutes(5).ToBinary();
            
            var allowedUpdateTimeAsBinary =  
                currentUserData.LastUserCurrencyUpdateTimeBinary + fiveMinutesAsBinary;

            var currentTimeAsBinary = DateTime.UtcNow.ToBinary();

            if(allowedUpdateTimeAsBinary <= currentTimeAsBinary)
            {
                currentUserData.AddUserCurrency(randomNumber);
                currentUserData.SetLastCurrencyUpdateAsBinary(currentTimeAsBinary);
            }
        }
    }
}