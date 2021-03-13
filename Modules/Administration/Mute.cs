using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BunniBot.Database;
using BunniBot.Database.Models;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BunniBot.Modules.Administration
{
    public class Mute : ModuleBase<SocketCommandContext>
    {
        public async Task AddMute(SocketCommandContext context, SocketGuildUser mentionedUser, string reason)
        {
            var user = context.User as SocketGuildUser;

            if (user == null)
            {
                //todo: error handle
                return;
            }

            if (user.GuildPermissions.MuteMembers)
            {
                var mongoDbHandler = new MongoDBHandler(context.Guild.Id.ToString());
                var serverSettings =
                    mongoDbHandler.LoadRecordByField<ServerSettingsModel>("ServerSettings", "_id", "mute_role");

                if (serverSettings != null && serverSettings.Value != null)    
                {
                    var logHandler = new AdminLogHandler();
                    
                    await logHandler.AddLogAsync(
                        context.Guild.Id.ToString(), Convert.ToInt64(mentionedUser.Id), mentionedUser.Username, "Mute", reason);    
                    
                    await mentionedUser.AddRoleAsync(
                        context.Guild.GetRole(Convert.ToUInt64(serverSettings.Value)));
                    
                    
                    var builder = new EmbedBuilder();

                    builder.WithAuthor(user.Username, user.GetAvatarUrl());
                    builder.WithColor(255, 183, 229);
                    builder.WithCurrentTimestamp();
                    builder.WithTitle("A user has been muted!");
                    builder.AddField("Muted User", mentionedUser.Username, true);
                    builder.AddField("Muted User ID", mentionedUser.Id, true);
                    builder.AddField("Reason", reason);
                    
                    await context.Channel.SendMessageAsync("", false, builder.Build());
                }
                else
                {
                    var guildRoles = context.Guild.Roles.ToList();
                    var existingMuteRole = guildRoles.Find(role => role.Name == "muted");
                    if (existingMuteRole != null && user.GuildPermissions.Administrator)
                    {
                        await SetMuteRole(context, existingMuteRole);
                        await AddMute(context, mentionedUser, reason);
                    }
                    else
                    {
                        await context.Channel.SendMessageAsync(
                            "Could not mute the user. Please ask a administrator to set the mute role using !setmuterole @role");
                    }
                }
            }
            else
            {
                await context.Channel.SendMessageAsync("You are not allowed to mute a user!");
            }
        }
    
        public async Task SetMuteRole(SocketCommandContext context, SocketRole role)
        {
            var user = context.User as SocketGuildUser;

            if (user == null)
            {
                //todo: error handle
                return;
            }

            if (user.GuildPermissions.Administrator)
            {
                var roleName = new Dictionary<string, string>  {{"role_name", role.Name}} ;
                var serverSettings = new ServerSettingsModel("mute_role",Convert.ToInt64(role.Id), roleName);
                var mongoDbHandler = new MongoDBHandler(context.Guild.Id.ToString());
                await mongoDbHandler.Upsert("ServerSettings", serverSettings.GetId(), serverSettings);
            }
            else
            {
                await context.Channel.SendMessageAsync("Only administrators are allowed to set a mute role!");
            }
            
        }
    }
}