using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordTracker.Data
{
    public class RelatedVoiceEvent
    {
        public int Id { get; set; }
        public string InitalEvent { get; set; }
        public string SecondaryEvent { get; set; }
        public bool IsPrimary { get; set; }
    }
}
