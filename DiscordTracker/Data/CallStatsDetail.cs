using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordTracker.Data
{
    public class CallStatsDetail
    {
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        // minutes
        public int? Duration { get; set; }
        public string Channel { get; set; }
        public string User { get; set; }
        public string EventType { get; set; }
    }
}
