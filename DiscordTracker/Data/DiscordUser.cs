using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

        
    }
}