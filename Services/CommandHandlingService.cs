using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace BunniBot.Services
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        /// <summary>
        /// Sets the needed variables for the command handler and listens to events
        /// </summary>
        /// <param name="services">Gives the services of the discord bot</param>
        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            //Event listeners
            _commands.CommandExecuted += CommandExecutedAsync;
            _discord.MessageReceived += MessageReceivedAsync;
        }

        /// <summary>
        /// Initializes the command handler.
        /// </summary>
        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        /// <summary>
        /// Recieves the messages send by the users in a discord guild
        /// </summary>
        /// <param name="rawMessage">Gives the raw message of a user.</param>
        private async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasCharPrefix('!', ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <param name="command">Gives the command</param>
        /// <param name="context">Gives the context needed to perform a command</param>
        /// <param name="result">Gives the result of th performed command.</param>
        private async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found)
            if (!command.IsSpecified)
            {
                await context.Channel.SendMessageAsync("I dont know that command! Please try !Help.");
                return;
            }

            // the command was executed successfully
            if (result.IsSuccess)
                return;

            // the command failed and notifies the user what went wrong
            await context.Channel.SendMessageAsync($"error: {result}");
        }
    }
}