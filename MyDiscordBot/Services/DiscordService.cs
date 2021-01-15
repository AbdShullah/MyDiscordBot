using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyDiscordBot.Commands;
using Serilog;

namespace MyDiscordBot.Services
{
    public class DiscordService
    {
        private readonly ILogger<DiscordService> _logger;

        public DiscordService(
            IServiceProvider services,
            ILoggerFactory loggerFactory,
            IOptions<DiscordOptions> options,
            ILogger<DiscordService> logger)
        {
            _logger = logger;
            // Discord configuration
            var discordOptions = options.Value;
            var discordConfiguration = new DiscordConfiguration
            {
                Token = discordOptions.Token,
                ShardCount = discordOptions.ShardCount,
                MinimumLogLevel = discordOptions.LogLevel,
                Intents = discordOptions.Intents,
                LoggerFactory = loggerFactory.AddSerilog()
            };
            // Commands configuration
            var commandsConfiguration = new CommandsNextConfiguration
            {
                StringPrefixes = discordOptions.Prefixes,
                Services = services
            };
            // Interactivity configuration
            var interactivityConfiguration = new InteractivityConfiguration();

            // Initalize objects
            Discord = new DiscordClient(discordConfiguration);
            Commands = Discord.UseCommandsNext(commandsConfiguration);
            Interactivity = Discord.UseInteractivity(interactivityConfiguration);
            Commands.RegisterCommands(Assembly.GetExecutingAssembly());

            // Subscribe to events
            Commands.CommandErrored += CommandsOnCommandErrored;
        }

        public DiscordClient Discord { get; }
        public CommandsNextExtension Commands { get; }
        public InteractivityExtension Interactivity { get; }

        private async Task CommandsOnCommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            var embedBuilder = new DiscordEmbedBuilder()
                .WithTitle($"Command failed: {e.Command.Name}")
                .WithDescription($"{e.Exception.Message}")
                .WithTimestamp(DateTime.Now)
                .WithFooter(e.Context.User.Username, e.Context.User.AvatarUrl)
                .WithColor(Colors.Error);

            if (e.Exception is not CommandException)
                _logger.LogError("Command failed:\nName: {c}\nUser: {u}\nException: {e}",
                    e.Command.Name, e.Context.User.Username, e.Exception);
            await e.Context.RespondAsync(embed: embedBuilder);
        }
    }
}