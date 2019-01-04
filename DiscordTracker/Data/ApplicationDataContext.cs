using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordTracker.Data
{
    public class ApplicationDataContext : DbContext
    {
        public ApplicationDataContext(DbContextOptions options) : base(options)
        {
        }

        public ApplicationDataContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            optionsBuilder.UseSqlServer("Server=192.168.47.2;Database=BobBot-Dev;User Id=bob;Password=bob");
#else
            optionsBuilder.UseSqlServer("Server=192.168.47.2;Database=BobBot;User Id=bob;Password=bob");
#endif
        }

        internal DiscordUser Find(ulong id)
        {
            throw new NotImplementedException();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quote>().HasOne(q => q.Author).WithMany(u => u.QuotesAuthored).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Quote>().HasOne(q => q.AddedBy).WithMany(u => u.QuotesSaved).OnDelete(DeleteBehavior.Restrict);
        }


        public DbSet<VoiceEventLog> VoiceEventLog { get; set; }
        public DbSet<DiscordUser> DiscordUser { get; set; }
        public DbSet<DiscordVoiceChannel> DiscordVoiceChannel { get; set; }
        public DbSet<Setting> Settings{ get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<RelatedVoiceEvent>  RelatedVoiceEvents { get; set; }
    }
}
