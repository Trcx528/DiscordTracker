using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordTracker.Data
{
    public class UserMessageReaction
    {
        public int Id { get; set; }
        public ulong ReactorId { get; set; }
        public string Reaction { get; set; }
        public ulong MessageId { get; set; }
        public DateTime Created { get; set; }


        public DiscordMessage Message { get; set; }
    }
}
