using System.Threading.Tasks;
using Discord.Commands;


namespace BunniBot.Modules.Text
{ 
    internal class Ping : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Pong!
        /// </summary>
        /// <param name="context">Gives the context needed to execute the command.</param>
        public async Task ShowPing(SocketCommandContext context)
        {
            await context.Channel.SendMessageAsync("Pong!");
        }
            
    }
}