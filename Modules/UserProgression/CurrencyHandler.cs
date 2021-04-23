using System;
using System.Threading.Tasks;
using BunniBot.Database.Models;
using BunniBot.Services;
using Discord;
using Discord.Commands;

namespace BunniBot.Modules.UserProgression
{
    public class CurrencyHandler : ModuleBase<SocketCommandContext>
    {
        public void MessageRecieved(SocketCommandContext context)
        {
            var serverData = ServerDataManager.GetServerDataByServerId(context.Guild.Id);
            UserDataModel currentUserData;

            try
            { 
                currentUserData = serverData.GetUserDataModel()[context.User.Id];
            }
            catch(Exception)
            {
                currentUserData = new UserDataModel(context.User.Id, context.User.Username);
                currentUserData.LastUserCurrencyUpdateTimeBinary = 0;
                serverData.SetUserData(context.User.Id, currentUserData);
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

        public async Task ShowBalance(SocketCommandContext context)
        {
            var serverData = ServerDataManager.GetServerDataByServerId(context.Guild.Id);
             var userData = serverData.GetUserDataModel()[context.User.Id];
             
             var builder = new EmbedBuilder();

             builder.WithAuthor(context.User.Username, context.User.GetAvatarUrl());
             builder.WithTitle("Balance");
             builder.WithColor(255, 183, 229);
             builder.WithDescription("You have a balance of **¥" + userData.GetUserCurrency() + "**");

             await context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}