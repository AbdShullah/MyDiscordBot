using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyDiscordBot.Data;
using MyDiscordBot.Services;
using Serilog;

namespace MyDiscordBot
{
    public static class Startup
    {
        public static async Task StartAsync()
        {
            // Create Microsoft Service Collection
            var services = new ServiceCollection();
            ConfigureServices(services);
            await using var serviceProvider = services.BuildServiceProvider();
            var discord = serviceProvider.GetRequiredService<DiscordService>();
            await discord.Discord.ConnectAsync(new DiscordActivity($"{discord.Prefix} help", ActivityType.ListeningTo));
            await Task.Delay(Timeout.Infinite);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Build config
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json", false, true)
                .AddJsonFile("logging.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            // Configure logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration, "Serilog")
                .CreateLogger();
            // Configure Services
            services
                // Options
                .Configure<DiscordOptions>(configuration.GetSection(nameof(DiscordOptions)))
                // Singletons
                .AddSingleton<DiscordService>()
                // Database
                .AddEFSecondLevelCache(options => options.UseMemoryCacheProvider().DisableLogging(true))
                .AddDbContext<BotContext>((serviceProvider, options) => options
                    .UseSqlite(configuration.GetConnectionString("Default"))
                    .AddInterceptors(serviceProvider.GetRequiredService<SecondLevelCacheInterceptor>()))
                // Logger is built in DiscordService
                .AddLogging();
        }
    }
}