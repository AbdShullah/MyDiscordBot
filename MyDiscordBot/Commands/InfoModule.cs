using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace MyDiscordBot.Commands
{
    [Description("Information")]
    public class InfoModule : BaseCommandModule
    {
        [Command("info")]
        [Aliases("about")]
        [Description("Show information about this bot")]
        public async Task InformationCommand(CommandContext ctx)
        {
            var app = ctx.Client.CurrentApplication;
            var embedBuilder = new DiscordEmbedBuilder()
                .WithTitle(ctx.Client.CurrentUser.Username)
                .WithDescription(app.Description)
                .WithThumbnail(ctx.Client.CurrentUser.AvatarUrl)
                .WithColor(Colors.Information)
                .AddField("App owners:", string.Join(", ", app.Owners.Select(owner => owner.Username)))
                .AddField(".NET Version:", Environment.Version.ToString())
                .AddField("OS:", Environment.OSVersion.ToString())
                .AddField("CPU Count:", Environment.ProcessorCount.ToString())
                .AddField("GC RAM Usage", $"{GC.GetTotalMemory(true) / (1024 * 1024)} MB");

            await ctx.RespondAsync(embed: embedBuilder);
        }

        [Command("ping")]
        [Description("Check ping")]
        public async Task PingCommand(CommandContext ctx)
        {
            var msg = await ctx.RespondAsync("Ping!");
            var time = DateTime.Now;
            await msg.ModifyAsync("Pong!");
            var timespan = DateTime.Now - time;
            await ctx.RespondAsync($"Latency: {timespan.Milliseconds}");
        }
    }
}