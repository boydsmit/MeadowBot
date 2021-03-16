using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace BunniBot.Modules.Text
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Shows all the possible commands of this bot.
        /// </summary>
        /// <param name="context">Gives the context needed to perform the command.</param>
        public async Task ShowHelp(SocketCommandContext context)
        {  
           
            var builder = new EmbedBuilder();
            builder.WithAuthor(context.Client.CurrentUser.Username, context.Client.CurrentUser.GetAvatarUrl());
            builder.WithTitle("All my commands:");
            builder.WithColor(255, 183, 229);
            
            builder.AddField("Interactions",
                "!Hug" + Environment.NewLine +
                "!Kiss" + Environment.NewLine +
                "!Pat" + Environment.NewLine +
                "!Lick" + Environment.NewLine +
                "!Slap");

            builder.AddField("Administration",
                "!Ban" + Environment.NewLine +
                "!Kick" + Environment.NewLine +
                "!Warn" + Environment.NewLine +
                "!Unban" + Environment.NewLine +
                "!Mute" + Environment.NewLine +
                "!Unmute" + Environment.NewLine +
                "!Warnings");
            
            builder.AddField("Text",
                "!Help" +  Environment.NewLine +
                "!Ping");
            
            builder.WithFooter("Execute by using '!'   ex:'!help'");
            await context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}