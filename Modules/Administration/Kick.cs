using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BunniBot.Modules.Administration
{
    public class Kick : ModuleBase<SocketCommandContext>
    {
        public async Task Main(SocketCommandContext context, SocketGuildUser mentionedUser, string reason)
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
                    builder.AddField("Kicked user", "`" + mentionedUser + "`", true);
                    builder.AddField( "\u200b", "\u200b", true);
                    builder.AddField("Kicked user ID", "`" + mentionedUser.Id + "`", true);

                    if (reason != null)
                    {
                        builder.AddField("Reason", "`" + reason + "`"); 
                    }
                    
                    builder.WithCurrentTimestamp();
                    
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