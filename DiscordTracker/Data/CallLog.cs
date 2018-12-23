using Discord.WebSocket;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTracker.Data
{
    public class CallLog
    {
        public int Id { get; set; }
        public string Channel { get; set; }
        public string Username { get; set; }
        public DateTime JoinTime { get; set; }
        public DateTime? LeaveTime { get; set; }

        [NotMapped]
        public TimeSpan TotalTime { get => LeaveTime.HasValue ? LeaveTime.Value - JoinTime : new TimeSpan(0); }

        public static async Task JoinedAsync(SocketUser user, string channel)
        {
            var cl = new CallLog() { Username = user.Username, Channel = channel, JoinTime = DateTime.Now };
            Program._db.Add(cl);
            await Program._db.SaveChangesAsync();
        }

        public static async Task LeftAsync(SocketUser user, string channel)
        {
            var logs = Program._db.CallLogs.Where(cl => cl.Username == user.Username && cl.Channel == channel && cl.LeaveTime == null);
            foreach(var cl in logs)
            {
                cl.LeaveTime = DateTime.Now;
            }

            await Program._db.SaveChangesAsync();
        }
    }
}