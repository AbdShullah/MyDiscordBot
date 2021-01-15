using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using MyDiscordBot.Data;
using MyDiscordBot.Data.Models;

namespace MyDiscordBot.Commands
{
    [Description("Server settings")]
    [RequireGuild]
    public class GuildSettingsModule : BaseCommandModule
    {
        public BotContext Db { private get; init; }


        [Command("settings")]
        [Description("View server settings")]
        public async Task ServerSettingsCommand(CommandContext ctx)
        {
            var settings = await FindOrCreateSettingsCacheAsync(ctx.Guild.Id);

            var embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Sever settings")
                .WithDescription(ctx.Guild.Name)
                .WithColor(Colors.Information)
                .AddField("Prefix:", settings.Prefix ?? "Default");

            await ctx.RespondAsync(embed: embedBuilder);
        }

        [Command("prefix")]
        [Description("Set server prefix")]
        public async Task SetPrefixCommand(CommandContext ctx, string prefix = null)
        {
            if (prefix != null && prefix.Length >= 32)
                throw new CommandException("Prefix can't be longer than 32 characters.");

            var settings = await FindOrCreateSettingsAsync(ctx.Guild.Id);
            settings.Prefix = prefix;
            await Db.SaveChangesAsync();
            await ctx.RespondAsync($"Prefix set to: {prefix ?? "Default"}");
        }

        private async Task<GuildSettings> FindOrCreateSettingsAsync(ulong id)
        {
            var settings = await Db.GuildsSettings
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.GuildId == id);
            
            if (settings != null)
            {
                return settings;
            }
            settings = new GuildSettings() {GuildId = id};
            await Db.GuildsSettings.AddAsync(settings);
            return settings;
        }
        
        private async Task<GuildSettings> FindOrCreateSettingsCacheAsync(ulong id)
        {
            var settings = await Db.GuildsSettings
                .AsQueryable()
                .AsNoTracking()
                .Cacheable()
                .FirstOrDefaultAsync(x => x.GuildId == id);
            
            if (settings != null)
            {
                return settings;
            }
            settings = new GuildSettings() {GuildId = id};
            await Db.GuildsSettings.AddAsync(settings);
            await Db.SaveChangesAsync();
            return settings;
        }
    }
}