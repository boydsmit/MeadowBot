using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BunniBot.Database;
using BunniBot.Database.Models;
using BunniBot.Modules.Administration;
using BunniBot.Modules.UserProgression;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BunniBot.Services
{
    public class JobHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private Dictionary<ulong, ServerDataManager> _serverDataCache = new Dictionary<ulong, ServerDataManager>();


        public JobHandlingService(IServiceProvider service)
        {
            _discord = service.GetRequiredService<DiscordSocketClient>();
            _discord.Ready += Initialize;
            _discord.MessageReceived += MessageRecieveAsync;
        }
        
        public Task Initialize()
        {
            Task.Run(() => AutoJobRunner());
            Task.Run(() => LoadCache());
            return Task.CompletedTask;
        }

        public async Task MessageRecieveAsync(SocketMessage rawMessage)
        {
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;
            
            var context = new SocketCommandContext(_discord, message);
            
            var currencyHandler = new CurrencyHandler();
            currencyHandler.Initialize(context, ref _serverDataCache);
            currencyHandler.MessageRecieved();
        }
        
        private Task LoadCache()
        {
            foreach (var guild in _discord.Guilds)
            {   
                var serverDataManager = new ServerDataManager(guild.Id);
                var database = new MongoDBHandler(guild.Id.ToString());
                
                var userDataList = database.GetAllDocumentsFromTable<UserDataModel>("UserData");
                foreach (var userData in userDataList)
                {
                    serverDataManager.AddUserData(userData.UserId, userData);
                }
                
                var shopRoles =  database.GetAllDocumentsFromTable<ShopRoleModel>("ShopRoles");
                foreach (var shopRole in shopRoles)
                {
                    serverDataManager.AddShopRole(shopRole.GetRoleId(), shopRole);
                }
                
                var serverSettings = database.GetAllDocumentsFromTable<ServerSettingsModel>("ServerSettings");
                foreach (var serverSetting in serverSettings)
                {
                    serverDataManager.AddServerSettings(serverSetting.Id, serverSetting);
                }
                _serverDataCache.Add(guild.Id, serverDataManager);
            }
            return Task.CompletedTask;
        }
        
        private async Task AutoJobRunner()
        {
            while (true)
            {
                foreach (var guild in _discord.Guilds)
                {    
                    var mute = new Mute();
                    await mute.AutoUnmute(guild);
                }
                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }
    }
}