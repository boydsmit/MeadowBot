using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace BunniBot.Modules.Text
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        public async Task Main(SocketCommandContext context)
        {  
           
            var builder = new EmbedBuilder();
            builder.WithAuthor(context.Client.CurrentUser.Username, context.Client.CurrentUser.GetAvatarUrl());
            builder.WithTitle("All my commands:");
            builder.WithColor(255, 183, 229);
            
            builder.AddField("Text",
                "```" + Environment.NewLine +
                "!Help" +  Environment.NewLine +
                "!Ping```");
            
            builder.AddField("Administration",
                "```" + Environment.NewLine +
                "!Ban" + Environment.NewLine +
                "!Kick```");
            
            builder.WithFooter("Execute by using '!'   ex:'!help'");
            await context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}