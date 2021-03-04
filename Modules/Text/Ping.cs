using System.Threading.Tasks;
using Discord.Commands;


namespace BunniBot.Modules.Text
{ 
    internal class Ping : ModuleBase<SocketCommandContext>
    {
        public async Task Main(SocketCommandContext context)
        {
            await context.Channel.SendMessageAsync("Ping!");
        }
            
    }
}