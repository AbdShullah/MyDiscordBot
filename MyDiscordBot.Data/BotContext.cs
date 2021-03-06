﻿using Microsoft.EntityFrameworkCore;
using MyDiscordBot.Data.Models;

namespace MyDiscordBot.Data
{
    public sealed class BotContext : DbContext
    {
        public BotContext(DbContextOptions<BotContext> options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<GuildSettings> GuildsSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<GuildSettings>()
                .HasKey(x => x.GuildId);

            modelBuilder
                .Entity<GuildSettings>()
                .Property(x => x.GuildId)
                .HasConversion(x => (long) x, x => (ulong) x);

            base.OnModelCreating(modelBuilder);
        }
    }
}