using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyDiscordBot.Commands;
using MyDiscordBot.Data;
using Serilog;

namespace MyDiscordBot.Services
{
    public class DiscordService
    {
        private readonly BotContext _db;
        private readonly ILogger<DiscordService> _logger;

        public DiscordService(
            IServiceProvider services,
            BotContext db,
            ILoggerFactory loggerFactory,
            IOptions<DiscordOptions> options,
            ILogger<DiscordService> logger)
        {
            _db = db;
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
            Prefix = discordOptions.Prefix;
            var commandsConfiguration = new CommandsNextConfiguration
            {
                PrefixResolver = PrefixResolver,
                Services = services
            };
            // Interactivity configuration
            var interactivityConfiguration = new InteractivityConfiguration();

            // Initialize objects
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

        public string Prefix { get; init; }


        private Task<int> PrefixResolver(DiscordMessage message)
        {
            var prefix = GetServerPrefix(message.Channel.Guild);
            prefix ??= Prefix;
            return Task.FromResult(message.GetStringPrefixLength(prefix));
        }

        private string GetServerPrefix(DiscordGuild guild)
        {
            if (guild == null) return null;
            var settings = _db.GuildsSettings.Cacheable().FirstOrDefault(x => x.GuildId == guild.Id);
            return settings?.Prefix;
        }

        private async Task CommandsOnCommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            var embedBuilder = new DiscordEmbedBuilder()
                .WithTitle($"Command failed: {e.Command.Name}")
                .WithDescription($"{e.Exception.Message}")
                .WithTimestamp(DateTime.Now)
                .WithFooter(e.Context.User.Username, e.Context.User.AvatarUrl)
                .WithColor(Colors.Error);

            if (e.Exception is not CommandException)
                _logger.LogError(e.Exception,
                    "Command failed:\nName: {C}\nUser: {U}",
                    e.Command.Name, e.Context.User.Username);
            await e.Context.RespondAsync(embedBuilder);
        }
    }
}