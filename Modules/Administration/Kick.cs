using System;
using System.Threading.Tasks;
using BunniBot.Database;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BunniBot.Modules.Administration
{
    public class Kick : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Kicks a user from the server.
        /// </summary>
        /// <param name="context">Gives the needed context to execute the command.</param>
        /// <param name="mentionedUser">Gives the user that needs to be kicked.</param>
        /// <param name="reason">Gives a reason of why the user was kicked.</param>
        public async Task KickUser(SocketCommandContext context, SocketGuildUser mentionedUser, string reason)
        {
            var user = context.User as SocketGuildUser;
            if (user == null)
            {
                //todo: error handle
                return;
            }
            
            if (user.GuildPermissions.KickMembers)
            {
                if (!mentionedUser.GuildPermissions.Administrator)
                {
                    await mentionedUser.KickAsync(reason);
                    
                    var builder = new EmbedBuilder();
                    
                    builder.WithAuthor(user.Username, user.GetAvatarUrl());
                    builder.WithTitle("A user has been kicked!");
                    builder.WithColor(255, 183, 229); 
                    builder.AddField("Kicked user", mentionedUser, true);
                    builder.AddField( "\u200b", "\u200b", true);
                    builder.AddField("Kicked user ID", mentionedUser.Id, true);

                    if (reason != null)
                    {
                        builder.AddField("Reason", reason);
                    }
                    
                    builder.WithCurrentTimestamp();
                    
                    var adminLogHandler = new AdminLogHandler();
                    
                    //If no reason was given set the reason to 'no reason given'
                    reason = reason ??  "No reason given";
                    
                    //Adds an entry in the logs
                    await adminLogHandler.AddLogAsync(context.Guild.Id.ToString(),Convert.ToInt64(mentionedUser.Id), 
                        mentionedUser.Username, "Kick", reason);
                    
                    await context.Channel.SendMessageAsync("", false, builder.Build());
                }
                else
                {
                    await context.Channel.SendMessageAsync("You are not allowed to kick an Administrator.");
                }
            }
            else
            {
                await context.Channel.SendMessageAsync("You have insufficient permissions to perform this command.");
            }
        }
    }
}