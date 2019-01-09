using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTracker.Data
{
    public class Log
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public ulong? UserId { get; set; }
        public ulong? ChannelId { get; set;}
        public DateTime DateTime { get; set; }


        public static async Task LogAsync(SocketUser user, SocketVoiceChannel channel, string eventName)
        {
            await DiscordUser.CreateOrGetAsync(user);
            var log = new Log() { UserId = user.Id, ChannelId = channel.Id, DateTime = DateTime.Now };
            log.Message = $"{user.Username} {eventName} in {channel.Name}";
            Console.WriteLine($"{DateTime.Now} {log.Message}");
            Program._db.Add(log);
            await Program._db.SaveChangesAsync();
        }

        public static async Task LogAsync(SocketUser user, string message)
        {
            await DiscordUser.CreateOrGetAsync(user);
            var log = new Log() { UserId = user.Id, Message = message };
            Console.WriteLine($"{DateTime.Now} {log.Message}");
            Program._db.Add(log);
            await Program._db.SaveChangesAsync();
        }

        public static async Task LogAsync(string message)
        {

            var log = new Log() { DateTime = DateTime.Now, Message = message };
            Console.WriteLine($"{DateTime.Now} {log.Message}");
            Program._db.Add(log);
            await Program._db.SaveChangesAsync();
        }
    }
}
