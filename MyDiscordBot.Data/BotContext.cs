using Microsoft.EntityFrameworkCore;

namespace MyDiscordBot.Data
{
    public class BotContext : DbContext
    {
        public BotContext(DbContextOptions<BotContext> options) : base(options)
        {
        }
    }
}