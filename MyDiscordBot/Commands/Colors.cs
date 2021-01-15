using DSharpPlus.Entities;

namespace MyDiscordBot.Commands
{
    public static class Colors
    {
        public static DiscordColor Information { get; } = new(0x007FFF);
        public static DiscordColor Important { get; } = DiscordColor.Orange;
        public static DiscordColor Warning { get; } = DiscordColor.Brown;
        public static DiscordColor Error { get; } = DiscordColor.Red;
    }
}