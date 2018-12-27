using Discord.WebSocket;
using System;
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
            var vel = new VoiceEventLog() { Date = DateTime.Now, ChannelId = chan.Id, UserId = user.Id };
            Program._db.Add(vel);
            await Program._db.SaveChangesAsync();
        }
    }
}