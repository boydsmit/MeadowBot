using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BunniBot.Modules.Administration
{
    public class Ban : ModuleBase<SocketCommandContext>
    {
        public async Task  Main(SocketCommandContext context, IGuildUser mentionedUser, string reason)
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
                    builder.AddField("Banned user", "`" + mentionedUser + "`", true);
                    builder.AddField( "\u200b", "\u200b", true);
                    builder.AddField("Banned user ID", "`" + mentionedUser.Id + "`", true);
                    builder.AddField("Reason", "`" + reason + "`");
                    builder.WithCurrentTimestamp();
                    
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
    }
}