using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordTracker.Data
{
    public class Quote
    {
        public int Id { get; set; }
        public string QuoteText { get; set; }
        public DiscordUser AddedBy { get; set; }
        public ulong AddedById { get; set; }
        public DateTime Created { get; set; }
        public ulong AuthorId { get; set; }
        public DiscordUser Author { get; set; }
    }
}
