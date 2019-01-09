using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordTracker.Data
{
    public class DiscordUserEvent
    {
        public int Id { get; set; }
        public ulong UserId { get; set;}
        public DiscordUser User { get; set; }
        public DateTime EventTime { get; set; }
        public string Status { get; set; }
        public string Game { get; set; }
    }
}
