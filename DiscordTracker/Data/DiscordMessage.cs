using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordTracker.Data
{
    public class DiscordMessage
    {
        public ulong Id { get; set; }
        public string Text { get; set; }
        public ulong AuthorId { get; set; }
        public ulong ChannelId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime Created { get; set; }

        public DiscordChannel Channel { get; set; }
        public DiscordUser Author { get; set; }

        public virtual List<UserMessageReaction> Reactions { get; set; }

        public static async Task<DiscordMessage> CreateAsync(Discord.IMessage message)
        {
            await DiscordChannel.CreateOrGetAsync(message.Channel);
            using (var db = new ApplicationDataContext())
            {
                var dm = new DiscordMessage() { Id = message.Id, AuthorId = message.Author.Id, ChannelId = message.Channel.Id, Created = DateTime.Now, IsDeleted = false, Text = message.Content };
                db.Add(dm);
                await db.SaveChangesAsync();
                return dm;
            }
        }

        public static async Task<DiscordMessage> CreateOrGetAsync(Discord.IMessage message)
        {
            var db = new ApplicationDataContext();
            var dm = db.DiscordMessages.Where(m => m.Id == message.Id).FirstOrDefault();
            db.Dispose();
            if (dm == null)
            {
                await CreateAsync(message);
            }
            return dm;
        }
    }
}
