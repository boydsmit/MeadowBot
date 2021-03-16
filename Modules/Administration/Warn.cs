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
        /// <summary>
        /// Warns a user and adds a entry in the logs
        /// </summary>
        /// <param name="context">Gives the context needed to execute the command.</param>
        /// <param name="mentionedUser">Gives the user that needs to be warned.</param>
        /// <param name="reason">Gives a reason of why the user was warned.</param>
        public async Task AddWarn(SocketCommandContext context, SocketGuildUser mentionedUser, string reason)
        {
            var user = context.User as SocketGuildUser;

            if (user == null)
            {
                //todo: error handle
                return;
            }

            if (user.GuildPermissions.MuteMembers)
            {
                var adminLogHandler = new AdminLogHandler();
                await adminLogHandler.AddLogAsync(context.Guild.Id.ToString(), Convert.ToInt64(mentionedUser.Id),
                    mentionedUser.Username, "Warn", reason);

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
            else
            {
                await context.Channel.SendMessageAsync("You do not have the permissions to warn.");
            }
        }

        /// <summary>
        /// Gets the warnings of a certain user by page (maximum of 5 logs per page).
        /// </summary>
        /// <param name="context">Gives the context needed to execute the command.</param>
        /// <param name="mentionedUser">Gives the user of whom the warnings need to be checked.</param>
        /// <param name="page">Gives the page the user wants to view.</param>
        public async Task GetWarnings(SocketCommandContext context, SocketGuildUser mentionedUser, int page)
        {
            var mongoDbHandler = new MongoDBHandler(context.Guild.Id.ToString());
            var userLog =
                mongoDbHandler.LoadRecordByField<UserLogsModel>("Logs", "_id", Convert.ToInt64(mentionedUser.Id));


            if (userLog == null)
            {
                await context.Channel.SendMessageAsync("This user has no logs!");
                return;
            }

            var builder = new EmbedBuilder();

            builder.WithAuthor(mentionedUser.Username, mentionedUser.GetAvatarUrl());
            builder.WithColor(255, 183, 229);

            var actions = userLog.Actions;
            
            //Calculates the amount of possible pages
            var maxPages = Math.Ceiling(Convert.ToDouble(actions.Count) / 5) - 1;

            //Checks if the current given page is possible to reach
            page = page > maxPages ? (int) maxPages : page; 

            //Loops the logs of the given pages
            for (var i = page * 5; i < page * 5 + 5; i++) 
            {
                if (actions.Count > i)
                {
                    builder.AddField("Case: [" + i + "] - " + actions[i].GetActionType(), actions[i].GetReason());
                    continue;
                }
                break;
            }

            builder.WithFooter("Page " + (page + 1) + "/" + (maxPages + 1));
            await context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}