using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using BunniBot.Modules.Administration;
using BunniBot.Modules.Media;
using BunniBot.Modules.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BunniBot.Modules
{
    public class ModuleHandler : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Alias("commands")]
        public async Task Help()
        {
            var help = new Help();
            await help.Main(Context);
        }
        
        [Command("ping")]
        [Alias("pong", "hello")]
        public async Task PingAsync()
        {
            var ping = new Ping();
            await ping.Main(Context);
        }

        [Command("kick")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser mentionedUser, [Remainder] string reason = null)
        {
            var kick = new Kick();
            await kick.Main(Context, mentionedUser, reason);
        }

        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketGuildUser mentionedUser, [Remainder] string reason = "No reason was given")
        {
            var ban = new Ban();
            await ban.AddBan(Context, mentionedUser, reason);
        }

        [Command("unban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Unban(ulong userId)
        {
            var ban = new Ban();
            await ban.RemoveBan(Context, userId);
        }

        [Command("warn")]
        public async Task Warn(SocketGuildUser mentionedUser, [Remainder] string reason)
        {
            var warn = new Warn();
            await warn.AddWarn(Context, mentionedUser, reason);
        }

        [Command("warnings")]
        [Alias("logs")]
        public async Task GetWarnings(SocketGuildUser mentionedUser, int page = 1)
        {
            page = page <= 0 ? 1 : page;
            var warn = new Warn();
            await warn.GetWarnings(Context, mentionedUser, page - 1);
        }

        [Command("hug")]
        public async Task Hug(SocketGuildUser mentionedUser)
        {
            var interactions = new Interactions();
            await interactions.PostInteraction(Context, "hug", mentionedUser, "got hugged by");
        }
        
        [Command("pat")]
        public async Task Pat(SocketGuildUser mentionedUser)
        {
            var interactions = new Interactions();
            await interactions.PostInteraction(Context, "pat", mentionedUser, "got a pat from");
        }
        
        [Command("kiss")]
        [Alias("smooch")]
        public async Task Kiss(SocketGuildUser mentionedUser)
        {
            var interactions = new Interactions();
            await interactions.PostInteraction(Context, "kiss", mentionedUser, "got a kiss from");
        }
        
        
        [Command("slap")]
        [Alias("hit")]
        public async Task Slap(SocketGuildUser mentionedUser)
        {
            var interactions = new Interactions();
            await interactions.PostInteraction(Context, "slap", mentionedUser, "got slapped by");
        }
    }
}