using Discord;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTracker.Data
{
    public class DiscordChannel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }

        public virtual List<DiscordMessage> Messages { get; set; }


        public static async Task<DiscordChannel> CreateOrGetAsync(IMessageChannel channel)
        {
            var chan = Program._discordChannels.Where(c => c.Id == channel.Id).FirstOrDefault();
            if (chan == null)
            {
                var db = new ApplicationDataContext();
                chan = new DiscordChannel() { Id = channel.Id, Name = channel.Name};
                db.Add(chan);
                Program._discordChannels = await db.DiscordChannels.ToListAsync();
                await db.SaveChangesAsync();
                db.Dispose();
            }
            return chan;
        }
    }
}
