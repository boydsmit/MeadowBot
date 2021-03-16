using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BunniBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace BunniBot
{
    public class Program
    {
        private static void Main(string[] args)
        {
             new Program().MainAsync().GetAwaiter().GetResult();
        } 
        
        /// <summary>
        /// Initializes and logs in the discord bot
        /// </summary>
        private async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;
                
                await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("token"));
                await client.StartAsync();
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                await client.SetGameAsync("!Help | BOOST BUNNIE!");
                services.GetRequiredService<JobHandlingService>();
                await Task.Delay(Timeout.Infinite);
            }
        }
        
        /// <summary>
        /// Simple Log writer that displays the logs in the console window.
        /// </summary>
        /// <param name="log">Gives a log message.</param>
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }
        
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<JobHandlingService>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }
    }
}
