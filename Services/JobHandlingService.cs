using System;
using System.Threading.Tasks;
using BunniBot.Modules.Administration;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace BunniBot.Services
{
    public class JobHandlingService
    {
        private readonly DiscordSocketClient _discord;
        
        public JobHandlingService(IServiceProvider service)
        {
            _discord = service.GetRequiredService<DiscordSocketClient>();
            _discord.Ready += AutoJobRunner;
        }
        
        private async Task AutoJobRunner()
        {
            while (true)
            {
                foreach (var guild in _discord.Guilds)
                {    
                    var mute = new Mute();
                    await mute.AutoUnmute(guild);
                }
                await Task.Delay(TimeSpan.FromMinutes(5));
            }
        }
    }
}