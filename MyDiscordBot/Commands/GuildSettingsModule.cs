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
            var settings = await Db.GuildsSettings.Cacheable().FirstOrDefaultAsync(x => x.GuildId == ctx.Guild.Id);

            var embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Sever settings")
                .WithDescription(ctx.Guild.Name)
                .WithColor(Colors.Information)
                .AddField("Prefix:", settings?.Prefix ?? "Default");

            await ctx.RespondAsync(embed: embedBuilder);
        }

        [Command("prefix")]
        [Description("Set server prefix. If no prefix provided, sets prefix to default")]
        public async Task SetPrefixCommand(CommandContext ctx, [Description("Bot prefix")] string prefix = null)
        {
            if (prefix != null && prefix.Length >= 32)
                throw new CommandException("Prefix can't be longer than 32 characters.");

            var settings = await Db.GuildsSettings.FindAsync(ctx.Guild.Id);
            if (settings == null)
            {
                settings = new GuildSettings {GuildId = ctx.Guild.Id};
                await Db.AddAsync(settings);
            }

            settings.Prefix = prefix;
            await Db.SaveChangesAsync();

            await ctx.RespondAsync($"Prefix set to: {prefix ?? "Default"}");
        }
    }
}