using Microsoft.EntityFrameworkCore;
using MyDiscordBot.Data.Models;

namespace MyDiscordBot.Data
{
    public static class BotContextExtensions
    {
        public static T FindOrCreate<T>(this DbSet<T> dbSet, T alternative, params object[] keyValues) where T : class
        {
            var settings = dbSet.Find(keyValues);
            if (settings != null) return settings;
            settings = alternative;
            dbSet.Add(settings);
            return settings;
        }
    }
}