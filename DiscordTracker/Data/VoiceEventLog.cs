using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTracker.Data
{
    public class VoiceEventLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public ulong UserId { get; set; }
        public string EventType { get; set; }
        public ulong ChannelId { get; set; }

        public DiscordVoiceChannel Channel { get; set; }
        public DiscordUser User { get; set; }


        public async static Task Log(SocketUser user, SocketChannel chan, string eventName)
        {
            if (Program._discordUsers.Where(u => u.User.Id == user.Id).FirstOrDefault() == null)
            {
                var du = new DiscordUser() { Id = user.Id, IsAdmin = false, Username = user.Username };
                Program._db.Add(du);
                await Program._db.SaveChangesAsync();
                Program._discordUsers = await Program._db.DiscordUser.ToListAsync();
            }

            if (Program._discordVoiceChannels.Where(v => v.Id == chan.Id).FirstOrDefault() == null)
            {
                var v = new DiscordVoiceChannel() { Id = chan.Id, Name = chan.ToString()};
                Program._db.Add(v);
                await Program._db.SaveChangesAsync();
                Program._discordVoiceChannels = await Program._db.DiscordVoiceChannel.ToListAsync();
            }

            var vel = new VoiceEventLog() { Date = DateTime.Now, ChannelId = chan.Id, UserId = user.Id, EventType = eventName };
            Program._db.Add(vel);
            await Program._db.SaveChangesAsync();
        }
    }
}