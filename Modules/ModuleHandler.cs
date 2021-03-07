using System;
using System.Threading.Tasks;
using BunniBot.Modules.Administration;
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
        public async Task Kick(SocketGuildUser mentionedUser, string reason = null)
        {
            var kick = new Kick();
            await kick.Main(Context, mentionedUser, reason);
        }

        [Command("ban")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task Ban(SocketGuildUser mentionedUser, [Remainder] string reason = "No reason was given")
        {
            var ban = new Ban();
            await ban.Main(Context, mentionedUser, reason);
        }

        [Command("warn")]
        public async Task Warn(SocketGuildUser mentionedUser, [Remainder] string reason)
        {
            var warn = new Warn();
            await warn.AddWarn(Context, mentionedUser, reason);
        }
        
    }
}