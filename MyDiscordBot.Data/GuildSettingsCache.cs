using Microsoft.Extensions.Caching.Memory;
using MyDiscordBot.Data.Models;

namespace MyDiscordBot.Data
{
    public class GuildSettingsCache
    {
        private readonly IMemoryCache _cache;
        private readonly BotContext _db;

        public GuildSettingsCache(BotContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public GuildSettings GetSettings(ulong id)
        {
            return _cache.GetOrCreate(id, _ => _db.GuildsSettings.Find(id) ?? new GuildSettings {GuildId = id});
        }

        public void SetSettings(GuildSettings guildSettings)
        {
            _cache.Set(guildSettings.GuildId, guildSettings);
            var settings = _db.GuildsSettings.Find(guildSettings.GuildId);
            if (settings == null)
                _db.GuildsSettings.Add(guildSettings);
            else
                _db.Entry(settings).CurrentValues.SetValues(guildSettings);
            _db.SaveChanges();
        }
    }
}