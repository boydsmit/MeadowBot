using System;
using System.Threading.Tasks;
using BunniBot.Database;
using BunniBot.Database.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BunniBot.Modules.Administration
{
    public class Warn : ModuleBase<SocketCommandContext>
    {
        public async Task AddWarn(SocketCommandContext context, SocketGuildUser mentionedUser, string reason)
        {
            var adminLogHandler =  new AdminLogHandler();
            await adminLogHandler.AddLogAsync(Convert.ToInt64(mentionedUser.Id), mentionedUser.Username,  "Warn", reason);
            
            var user = context.User as SocketGuildUser;

            if (user == null)
            {
                //todo: error handle
                return;
            }

            var builder = new EmbedBuilder();

            builder.WithAuthor(user.Username, user.GetAvatarUrl());
            builder.WithTitle("A user has been warned");
            builder.WithColor(255, 183, 229);
            builder.AddField("Warned User", mentionedUser.Username, true);
            builder.AddField("Warned User ID", mentionedUser.Id, true);
            builder.AddField("Reason", reason);
            builder.WithCurrentTimestamp();

            await context.Channel.SendMessageAsync("", false, builder.Build());
        }

        public async Task GetWarnings(SocketCommandContext context, SocketGuildUser mentionedUser, int page)
        {
            var mongoDbHandler = new MongoDBHandler("UserLogs");
            var userLog = mongoDbHandler.LoadRecordById<UserLogsModel>("Logs", Convert.ToInt64(mentionedUser.Id));

            if (userLog == null)
            {
                await context.Channel.SendMessageAsync("This user has no logs!");
                return;
            }

            var builder = new EmbedBuilder();

            builder.WithAuthor(mentionedUser.Username, mentionedUser.GetAvatarUrl());
            builder.WithColor(255, 183, 229);
            
            var actions = userLog.Actions;
            var maxPages = Math.Ceiling(Convert.ToDouble(actions.Count) / 5) - 1;    
            
            page = page > maxPages ? (int) maxPages : page;
            
            for (var i = page * 5; i < page * 5 + 5; i++)
            {
                if (actions[i] != null)
                {
                    builder.AddField("Case: [" + i + "] - " + actions[i].GetActionType(), actions[i].GetReason());
                }
                break;
            }

            builder.WithFooter("Page " + (page + 1) + "/" + (maxPages + 1));
            await context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
    
    
}