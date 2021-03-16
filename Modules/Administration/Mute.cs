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
        /// <summary>
        /// Mutes a user for a set time period.
        /// </summary>
        /// <param name="context">Gives the needed context to execute the command.</param>
        /// <param name="mentionedUser">Gives the user that needs to be muted.</param>
        /// <param name="mutePeriod">Gives the time period of the mute.</param>
        /// <param name="reason">Gives the reason of why the user was muted.</param>
        public async Task AddMute(SocketCommandContext context, SocketGuildUser mentionedUser, string mutePeriod,
            string reason)
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

                    //Adds an entry in the logs
                    await logHandler.AddLogAsync(
                        context.Guild.Id.ToString(), Convert.ToInt64(mentionedUser.Id), mentionedUser.Username, "Mute",
                        reason);

                    //Gives the role to the user to mute them.
                    await mentionedUser.AddRoleAsync(
                        context.Guild.GetRole(Convert.ToUInt64(serverSettings.Value)));
                    
                    var timeInt = mutePeriod.Replace(mutePeriod.Last(), ' ');
                    if (!int.TryParse(timeInt, out var result))
                    {
                        await context.Channel.SendMessageAsync("Given time was false! Usage: !Mute user 12D reason");
                        return;
                    }

                    var muteExpireTime = DateTime.Now;
                    switch (mutePeriod.Last())
                    {
                        case 's':
                            muteExpireTime = muteExpireTime.AddSeconds(result);
                            break;
                        case 'm':
                            muteExpireTime = muteExpireTime.AddMinutes(result);
                            break;
                        case 'h':
                            muteExpireTime = muteExpireTime.AddHours(result);
                            break;
                        case 'd':
                            muteExpireTime = muteExpireTime.AddDays(result);
                            break;
                        default:
                            await context.Channel.SendMessageAsync("Only seconds, minutes, hours or days are allowed!");
                            return;
                    }

                    var muteData = new MuteDataModel(true, mutePeriod, muteExpireTime.ToBinary());
                    var userData = new UserDataModel(mentionedUser.Id, mentionedUser.Username);
                    userData.SetMuteData(muteData);
                    
                    //Gives the user a muted state in the database
                    await mongoDbHandler.Upsert("UserData", userData.UserId, userData);

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
                else //Database entry of ServerSettings did not contain a role to mute members
                {
                    var guildRoles = context.Guild.Roles.ToList();
                    
                    //Checks if the server already contains a mute role
                    var existingMuteRole = guildRoles.Find(role => role.Name == "muted");
                    if (existingMuteRole != null && user.GuildPermissions.Administrator)
                    {
                        //Adds the role to the server settings and reruns the command
                        await SetMuteRole(context, existingMuteRole);
                        await AddMute(context, mentionedUser, mutePeriod, reason);
                    }
                    else //Server did not contain a mute role
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
        
        /// <summary>
        /// Sets the muted role in the database.
        /// </summary>
        /// <param name="context">Gives the context needed to execute the command.</param>
        /// <param name="role">Gives a discord role.</param>
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
                var roleName = new Dictionary<string, string> {{"role_name", role.Name}};
                var serverSettings = new ServerSettingsModel("mute_role", Convert.ToInt64(role.Id), roleName);
                var mongoDbHandler = new MongoDBHandler(context.Guild.Id.ToString());
                await mongoDbHandler.Upsert("ServerSettings", serverSettings.GetId(), serverSettings);
            }
            else
            {
                await context.Channel.SendMessageAsync("Only administrators are allowed to set a mute role!");
            }
        }
        
        /// <summary>
        /// Unmutes a given user.
        /// </summary>
        /// <param name="context">Gives the context needed to execute the command.</param>
        /// <param name="unmuteUser">Gives the user that needs to be unmuted.</param>
        public async Task Unmute(SocketCommandContext context, SocketGuildUser unmuteUser)
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
                var serverSettingsModel =
                    mongoDbHandler.LoadRecordByField<ServerSettingsModel>("ServerSettings", "_id", "mute_role");

                var roleId = Convert.ToUInt64(serverSettingsModel.GetValue());
                if (unmuteUser.Roles.Any(role => role.Id == roleId))
                {
                    await unmuteUser.RemoveRoleAsync(context.Guild.GetRole(roleId));

                    var builder = new EmbedBuilder();
                    builder.WithAuthor(unmuteUser.Username, unmuteUser.GetAvatarUrl());
                    builder.WithTitle("User has been unmuted!");
                    builder.WithColor(255, 183, 229);
                    builder.AddField("Unmuted User", unmuteUser.Username, true);
                    builder.AddField("Unmuted User ID", unmuteUser.Id, true);
                    
                    //Removes the muted state from the users database entry
                    var mutedUser = mongoDbHandler.LoadRecordByField<UserDataModel>("UserData", "_id", unmuteUser.Id);
                    mutedUser.SetMuteData(null);
                    await mongoDbHandler.Upsert("UserData", mutedUser.UserId, mutedUser);

                    await context.Channel.SendMessageAsync("", false, builder.Build());
                }
                else
                {
                    await context.Channel.SendMessageAsync("This user is not muted!");
                }
            }
            else
            {
                await context.Channel.SendMessageAsync("You do not have the permissions to mute a user!");
            }
        }
        
        /// <summary>
        /// Auto unmutes the a user if the mute timer has expired.
        /// </summary>
        /// <param name="guild">Gives the current guild that is being checked.</param>
        public async Task AutoUnmute(IGuild guild)
        {
            var mongoDbHandler = new MongoDBHandler(guild.Id.ToString());
            var mutedUsersInfo =
                mongoDbHandler.LoadAllRecordsWhereFieldExist<UserDataModel>("UserData", "MuteData");
            
            //Checks if the server has any muted users
            if (mutedUsersInfo == null)
            {
                return;
            }

            var muteRole =
                mongoDbHandler.LoadRecordByField<ServerSettingsModel>("ServerSettings", "_id", "mute_role");

            //Loops through every muted user
            foreach (var mutedUser in mutedUsersInfo)
            {
                var muteData = mutedUser.GetMuteData();

                //Checks if the mute timer has already expired
                if (DateTime.Now.ToBinary() >= muteData.GetUnmuteTimeAsBinary())
                {
                    var muteRoleId = Convert.ToUInt64(muteRole.Value);
                    var discordUser = await guild.GetUserAsync(mutedUser.GetUserId()) as SocketGuildUser;

                    //Checks if the user is still in the server
                    if (discordUser != null)
                    {
                        //Checks if the user has the muted role
                        if (discordUser.Roles.Any(role => role.Id == muteRoleId))
                        {
                            await discordUser.RemoveRoleAsync(guild.GetRole(muteRoleId));
                            
                            //Removes the muted state from the users database entry
                            mutedUser.SetMuteData(null);
                            await mongoDbHandler.Upsert("UserData", mutedUser.UserId, mutedUser);
                        }
                        else //User no longer has the muted role
                        {
                            //Removes the muted state from the users database entry
                            mutedUser.SetMuteData(null);
                            await mongoDbHandler.Upsert("UserData", mutedUser.UserId, mutedUser);
                        }
                    }
                    else //User has left the server
                    {
                        //Entirely removes the users database entry since they left the server and didnt join back even though the timer expired
                        await mongoDbHandler.DeleteDocumentByField<UserDataModel>("UserData", "_id", mutedUser.UserId);
                    }
                }
            }
        }
    }
}