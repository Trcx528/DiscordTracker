using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTracker.Data
{
    public class DiscordVoiceChannel
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public Discord.WebSocket.SocketChannel VoiceChannel { get => Program._client.GetChannel(this.Id); }

        public static async Task<DiscordVoiceChannel> CreateOrGetAsync(Discord.WebSocket.SocketChannel chan)
        {
            var vc = Program._discordVoiceChannels.Where(v => v.Id == chan.Id).FirstOrDefault();
            if (vc == null)
            {
                vc = new DiscordVoiceChannel() { Id = chan.Id, Name = chan.ToString() };
                Program._db.Add(vc);
                await Program._db.SaveChangesAsync();
                Program._discordVoiceChannels = await Program._db.DiscordVoiceChannel.ToListAsync();
            }

            return vc;
        }
    }
}