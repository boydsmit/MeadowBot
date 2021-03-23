using System.Collections.Generic;
using System.Threading.Tasks;
using BunniBot.Modules.Administration;
using BunniBot.Modules.Media;
using BunniBot.Modules.Text;
using BunniBot.Modules.UserProgression;
using BunniBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BunniBot.Modules
{
    public class ModuleHandler : ModuleBase<SocketCommandContext>
    {
        private static Dictionary<ulong, ServerDataManager> _serverDataCache = new Dictionary<ulong, ServerDataManager>();

        public void SetCache(ref Dictionary<ulong, ServerDataManager> serverDataCache)
        {
            _serverDataCache = serverDataCache;
        }
        
        [Command("help")]
        [Alias("commands")]
        public async Task Help()
        {
            var help = new Help();
            await help.ShowHelp(Context);
        }
        
        [Command("ping")]
        [Alias("pong", "hello")]
        public async Task PingAsync()
        {
            var ping = new Ping();
            await ping.ShowPing(Context);
        }

        [Command("kick")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser mentionedUser, [Remainder] string reason = null)
        {
            var kick = new Kick();
            await kick.KickUser(Context, mentionedUser, reason);
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

        [Command("mute")]
        public async Task Mute(SocketGuildUser mentionedUser, string mutePeriod,   [Remainder] string reason)
        {
            var mute = new Mute();
            await mute.AddMute(Context, mentionedUser, mutePeriod, reason, _serverDataCache[Context.Guild.Id]);
        }

        [Command("unmute")]
        public async Task Unmute(SocketGuildUser mentionedUser)
        {
            var mute = new Mute();
            await mute.Unmute(Context, mentionedUser, _serverDataCache[Context.Guild.Id]);
        }

        [Command("setmuterole")]
        public async Task SetMuteRole(SocketRole role)
        {
            var mute = new Mute();
            await mute.SetMuteRole(Context, role, _serverDataCache[Context.Guild.Id]);
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

        [Command("lick")]
        public async Task Lick(SocketGuildUser mentionedUser)
        {
            var interactions = new Interactions();
            await interactions.PostInteraction(Context, "lick", mentionedUser, "got licked by");
        }        

        [Command("d bump")]
        public async Task BumpReactor()
        {
            await Context.Channel.SendMessageAsync("Don't forget to vote for our server too! https://top.gg/servers/696727476692451430/vote");
        }

        [Command("BuyRole")]
        public async Task BuyRole(SocketRole role)
        {
            var currencyHandler = new CurrencyHandler();
            await currencyHandler.BuyItem(Context, role);
        }
    }
}