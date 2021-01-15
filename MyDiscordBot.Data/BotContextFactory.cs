using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MyDiscordBot.Data
{
    public class BotContextFactory : IDesignTimeDbContextFactory<BotContext>
    {
        public BotContext CreateDbContext(string[] args)
        {
            // Configuration
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<BotContext>()
                .UseSqlite(configuration.GetConnectionString("Default"));

            return new BotContext(optionsBuilder.Options);
        }
    }
}