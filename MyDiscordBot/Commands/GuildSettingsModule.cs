using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MyDiscordBot.Data;

namespace MyDiscordBot.Commands
{
    [Description("Server settings")]
    [RequireGuild]
    public class GuildSettingsModule : BaseCommandModule
    {
        public GuildSettingsCache GuildSettingsCache { private get; init; }


        [Command("settings")]
        [Description("View server settings")]
        public async Task ServerSettingsCommand(CommandContext ctx)
        {
            var settings = GuildSettingsCache.GetSettings(ctx.Guild.Id);

            var embedBuilder = new DiscordEmbedBuilder()
                .WithTitle("Sever settings")
                .WithDescription(ctx.Guild.Name)
                .WithColor(Colors.Information)
                .AddField("Prefix:", settings.Prefix ?? "Default");

            await ctx.RespondAsync(embed: embedBuilder);
        }

        [Command("prefix")]
        [Description("Set server prefix")]
        public async Task SetPrefixCommand(CommandContext ctx, [Description("Bot prefix")] string prefix = null)
        {
            if (prefix != null && prefix.Length >= 32)
                throw new CommandException("Prefix can't be longer than 32 characters.");

            var settings = GuildSettingsCache.GetSettings(ctx.Guild.Id);
            settings.Prefix = prefix;
            GuildSettingsCache.SetSettings(settings);
            await ctx.RespondAsync($"Prefix set to: {prefix ?? "Default"}");
        }
    }
}