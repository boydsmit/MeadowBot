using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BunniBot.Database;
using BunniBot.Database.Models;
using Discord.Commands;
using Discord.WebSocket;

namespace BunniBot.Modules.Administration
{
    public class Warn : ModuleBase<SocketCommandContext>
    {
        public async Task AddWarn(SocketCommandContext context, SocketGuildUser user, string reason)
        {
            var mongoDbHandler = new MongoDBHandler("UserLogs");
            var actionModel = new ActionsModel("Warn", reason);
            
            
            var currentEntry = mongoDbHandler.LoadRecordById<UserLogsModel>("Logs", Convert.ToInt64(user.Id));
            var actions = new List<ActionsModel>();
            if (currentEntry != null)
            {
                actions = currentEntry.Actions;
            }
            
            actions.Add(actionModel);
            
            var userLogsModel = new UserLogsModel(Convert.ToInt64(user.Id), actions);
            
            await mongoDbHandler.Upsert("Logs", userLogsModel.UserId, userLogsModel);
            await context.Channel.SendMessageAsync("User has been warned");
        }
    }
}