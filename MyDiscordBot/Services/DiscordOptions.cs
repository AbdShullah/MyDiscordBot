using DSharpPlus;
using Microsoft.Extensions.Logging;

namespace MyDiscordBot.Services
{
    public class DiscordOptions
    {
        public string Token { get; set; }
        public int ShardCount { get; set; } = 1;
        public DiscordIntents Intents { get; set; } = DiscordIntents.AllUnprivileged;
        public LogLevel LogLevel { get; set; } = LogLevel.Information;
        public string Prefix { get; set; }
    }
}