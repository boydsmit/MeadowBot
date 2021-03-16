using System;
using System.Threading.Tasks;
using BunniBot.Database;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BunniBot.Modules.Administration
{
    public class Ban : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Adds a user to the banned user list and removes them from the server.
        /// </summary>
        /// <param name="context">Gives the needed context of the executed command.</param>
        /// <param name="mentionedUser">Gives the user that needs to be banned.</param>
        /// <param name="reason">Gives a reason why the user has been banned.</param>
        public async Task  AddBan(SocketCommandContext context, IGuildUser mentionedUser, string reason)
        {
            var user = context.User as SocketGuildUser;

            if (user == null)
            {
                //todo: error handle
                return;
            }
            
            if (user.GuildPermissions.BanMembers)
            {
                if (!mentionedUser.GuildPermissions.Administrator)
                {
                    await context.Guild.AddBanAsync(mentionedUser, 0, reason);
                    var builder = new EmbedBuilder();
                    
                    builder.WithAuthor(user.Username, user.GetAvatarUrl());
                    builder.WithTitle("A user has been Banned!");
                    builder.WithColor(255, 183, 229);
                    builder.AddField("Banned user", mentionedUser, true);
                    builder.AddField( "\u200b", "\u200b", true);
                    builder.AddField("Banned user ID", mentionedUser.Id, true);
                    builder.AddField("Reason", reason);
                    builder.WithCurrentTimestamp();
                    
                    var adminLogHandler = new AdminLogHandler();
                    
                    //Adds an entry in the logs
                    await adminLogHandler.AddLogAsync(context.Guild.Id.ToString(),Convert.ToInt64(mentionedUser.Id), 
                        mentionedUser.Username, "Ban", reason);

                    await context.Channel.SendMessageAsync("" , false, builder.Build());
                }
                else
                {
                    await context.Channel.SendMessageAsync("You are not allowed to ban an Administrator.");
                }
            }
            else
            {
                await context.Channel.SendMessageAsync("You have insufficient permissions to perform this command.");
            }
        }

        /// <summary>
        /// Removes a user from the banned user list.
        /// </summary>
        /// <param name="context">Gives the context needed to execute the command.</param>
        /// <param name="userId">Gives the id of the user that needs to be unbanned.</param>
        public async Task RemoveBan(SocketCommandContext context, ulong userId)
        {
            var user = context.User as SocketGuildUser;

            if (user == null)
            {
                //todo: error handle
                return;
            }

            if (user.GuildPermissions.BanMembers)
            {
                var ban = await context.Guild.GetBanAsync(userId);
                
                if (ban != null)
                {
                    await context.Guild.RemoveBanAsync(userId);
                    await context.Channel.SendMessageAsync("User has been unbanned.");
                    return;
                }
                await context.Channel.SendMessageAsync("User with id "  + userId + " is not banned.");
            }
        }
    }
}