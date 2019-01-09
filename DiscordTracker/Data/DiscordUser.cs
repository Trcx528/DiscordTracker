using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordTracker.Data
{
    public class DiscordUser
    {
        public ulong Id { get; set; }
        public string Username { get; set; }
        public string IRLName { get; set; }
        public bool IsAdmin { get; set; }

        public virtual List<Quote> QuotesSaved { get; set; }
        public virtual List<Quote> QuotesAuthored { get; set; }


        [NotMapped]
        public Discord.WebSocket.SocketUser User { get => Program._client.GetUser(Id); }

        public static async Task<DiscordUser> CreateOrGetAsync (Discord.WebSocket.SocketUser user)
        {
            var du = Program._discordUsers.Where(u => u.User.Id == user.Id).FirstOrDefault();
            if (du == null)
            {
                var db = new ApplicationDataContext();
                du = new DiscordUser() { Id = user.Id, IsAdmin = false, Username = user.Username, IRLName = user.Username };
                db.Add(du);
                Program._discordUsers = await db.DiscordUser.ToListAsync();
                db.Dispose();
            }
            return du;
    }


}
}