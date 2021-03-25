using System;
using System.Linq;
using System.Threading.Tasks;
using BunniBot.Database.Models;
using BunniBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BunniBot.Modules.UserProgression
{
    public class ShopRoleHandler : ModuleBase<SocketCommandContext>
    {
        private ServerDataManager _serverDataCache;
        private static SocketCommandContext _context;

        public void Initialize(ServerDataManager serverDataCache, SocketCommandContext context)
        {
            _serverDataCache = serverDataCache;
            _context = context;
        }

        public async Task AddShopRole(SocketRole role, int cost, int requiredLevel)
        {
            var user = _context.User as SocketGuildUser;

            if (user == null)
            {
                //todo: error handle
                return;
            }

            if (user.GuildPermissions.Administrator)
            {
                var shopRole = new ShopRoleModel(role.Id, role.Name, cost, requiredLevel);
                _serverDataCache.AddShopRole(role.Id, shopRole);

                var builder = new EmbedBuilder();

                builder.WithTitle("Added shop role!");
                builder.WithAuthor(user.Username, user.GetAvatarUrl());
                builder.WithColor(255, 183, 229);
                builder.AddField("Role id", role.Id);
                builder.AddField("Role name", role.Name);
                    
                builder.AddField("Role cost", cost);

                if (requiredLevel > 0)
                {
                    builder.AddField("Required level", requiredLevel);
                }
                await _context.Channel.SendMessageAsync("", false, builder.Build());
            }
            else
            {
                await _context.Channel.SendMessageAsync("Only admins can add shop roles!");
            }
        }

        public async Task BuyItem(SocketRole role)
        {
            var user = _context.User as SocketGuildUser;

            if (user == null)
            {
                //todo: error handle
                return;
            }

            try
            {
                var shopRole = _serverDataCache.GetShopRoleModel()[role.Id];
                var currentUserData = _serverDataCache.GetUserDataModel()[user.Id];

                if (user.Roles.Any(x => x.Id == shopRole.GetRoleId()))
                {
                    if (currentUserData.UserLevel >= shopRole.RequiredLevel)
                    {
                        if (currentUserData.UserCurrency >= shopRole.RoleCost)
                        {
                            currentUserData.SubtractUserCurrency(shopRole.RoleCost);
                            await user.AddRoleAsync(_context.Guild.GetRole(shopRole.GetRoleId()));

                            var builder = new EmbedBuilder();

                            builder.WithTitle("Bought shop role!");
                            builder.WithAuthor(user.Username, user.GetAvatarUrl());
                            builder.WithColor(0, 255, 0);
                            builder.AddField("Role name", role.Name);
                            builder.AddField("Role cost", shopRole.GetRoleCost());
                            builder.AddField("New balance", currentUserData.GetUserCurrency());

                            await _context.Channel.SendMessageAsync("", false, builder.Build());
                        }
                        else
                        {
                            var builder = new EmbedBuilder();
                            builder.WithAuthor(user.Username, user.GetAvatarUrl());
                            builder.WithColor(255, 0, 0);
                            builder.WithTitle("Insufficient funds!");
                            builder.AddField("Role cost", shopRole.GetRoleCost());
                            builder.AddField("Your funds", currentUserData.GetUserCurrency());
                            builder.AddField("Funds needed",
                                shopRole.GetRoleCost() - currentUserData.GetUserCurrency());

                            await _context.Channel.SendMessageAsync("", false, builder.Build());
                        }
                    }
                    else
                    {
                        var builder = new EmbedBuilder();
                        builder.WithAuthor(user.Username, user.GetAvatarUrl());
                        builder.WithColor(255, 0, 0);
                        builder.WithTitle("Your level is too low!");
                        builder.AddField("Role required level", shopRole.GetRequiredLevel());
                        builder.AddField("Your level", currentUserData.GetUserLevel());
                        builder.AddField("Levels needed", shopRole.GetRequiredLevel() - currentUserData.GetUserLevel());

                        await _context.Channel.SendMessageAsync("", false, builder.Build());
                    }
                }
                else
                {
                    await _context.Channel.SendMessageAsync("You already own this role!");
                }
            }
            catch (Exception)
            {
                await _context.Channel.SendMessageAsync("This role is not a shop role");
            }
        }

        public async Task GetShopRoleInfo(SocketRole role)
        {
            var user = _context.User as SocketGuildUser;

            if (user == null)
            {
                //todo: error handle
                return;
            }

            try
            {
                var shopRole = _serverDataCache.GetShopRoleModel()[role.Id];
                
                var builder = new EmbedBuilder();
                builder.WithAuthor(user.Username, user.GetAvatarUrl());
                builder.WithColor(255, 183, 229);
                builder.AddField("Role name", role.Name);
                builder.AddField("Role cost", shopRole.GetRoleCost());
                builder.AddField("Required level", shopRole.GetRequiredLevel());

                await _context.Channel.SendMessageAsync("", false, builder.Build());
            }
            catch(Exception)
            {
                await _context.Channel.SendMessageAsync("This role is not a shop role");
            }
        }

        public async Task GetAllShopRoles()
        {
            var user = _context.User as SocketGuildUser;

            if (user == null)
            {
                return;
            }
            
            var shopRoles = _serverDataCache.GetShopRoleModel();
            
            var builder = new EmbedBuilder();
            builder.WithAuthor(user.Username, user.GetAvatarUrl());
            builder.WithColor(255, 183, 229);
            
            var embedField = new EmbedFieldBuilder();
            embedField.WithName("Shop Roles");
            
            //todo: add page system
            foreach (var shopRole in shopRoles)
            {
                embedField.Value += _context.Guild.GetRole(shopRole.Value.GetRoleId()).Mention + "\n";
            }
            builder.AddField(embedField);

            await _context.Channel.SendMessageAsync("", false, builder.Build());
        }
    }
}