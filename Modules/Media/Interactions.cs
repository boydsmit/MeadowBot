using System;
using System.Threading.Tasks;
using BunniBot.Database;
using BunniBot.Database.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BunniBot.Modules.Media
{
    public class Interactions : ModuleBase<SocketCommandContext>
    {
        /// <summary>
        /// Sends a interaction in the form of a type of media.
        /// </summary>
        /// <param name="context">Gives the context needed to execute the command.</param>
        /// <param name="type">Gives the form of interaction a user wants to perform.</param>
        /// <param name="mentionedUser">Gives the user the interaction is performed on.</param>
        /// <param name="actionLine">Gives a line that describes what interaction has been performed.</param>
        public async Task PostInteraction(SocketCommandContext context, string type, SocketGuildUser mentionedUser ,string actionLine)
        {
            var mongoDbHandler = new MongoDBHandler("MediaStorage");

            //Gets a random media from the database of the given interaction type
            var mediaStorage = mongoDbHandler.LoadRecordByField<MediaModel>("Interactions", "MediaType", type);
            var random = new Random();
            var index = random.Next(0, mediaStorage.MediaItems.Length);
            
            var user = context.User as SocketGuildUser;

            if (user == null)
            {
                //todo: error handle
                return;
            }
            
            var builder = new EmbedBuilder();
            
            builder.WithColor(255, 183, 229);
            builder.WithImageUrl(mediaStorage.MediaItems[index]);
            builder.WithDescription("**" + mentionedUser.Username + "** " + actionLine + " **" + user.Username + "**");
            await context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}