using System.Linq;
using EFCoreSecondLevelCacheInterceptor;
using MyDiscordBot.Data.Models;

namespace MyDiscordBot.Data
{
    public class GuildSettingsCache
    {
        private readonly BotContext _db;

        public GuildSettingsCache(BotContext db)
        {
            _db = db;
        }

        public GuildSettings GetSettings(ulong id)
        {
            return _db.GuildsSettings
                .AsQueryable()
                .Cacheable()
                .FirstOrDefault(x => x.GuildId == id) ?? new GuildSettings() {GuildId = id};
        }

        public void SetSettings(GuildSettings guildSettings)
        {
            var settings = _db.GuildsSettings.Find(guildSettings.GuildId);
            if (settings == null)
                _db.GuildsSettings.Add(guildSettings);
            else
                _db.Entry(settings).CurrentValues.SetValues(guildSettings);
            _db.SaveChanges();
        }
    }
}