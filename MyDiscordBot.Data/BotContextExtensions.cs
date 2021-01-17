using Microsoft.EntityFrameworkCore;
using MyDiscordBot.Data.Models;

namespace MyDiscordBot.Data
{
    public static class BotContextExtensions
    {
        public static GuildSettings FindOrCreate(this DbSet<GuildSettings> dbSet, ulong id)
        {
            var settings = dbSet.Find(id);
            if (settings != null) return settings;
            settings = new GuildSettings {GuildId = id};
            dbSet.Add(settings);
            return settings;
        }
    }
}