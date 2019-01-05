using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordTracker.Data
{
    public class CallStats
    {
        public string User { get; set; }
        /// <summary>
        /// Hours
        /// </summary>
        public decimal TimeInCall { get; set; }
        /// <summary>
        /// Hours
        /// </summary>
        public decimal TimeMuted { get; set; }
        /// <summary>
        /// Hours
        /// </summary>
        public decimal TimeDeafened { get; set; }
    }
}
