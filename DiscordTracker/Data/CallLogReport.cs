using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiscordTracker.Data
{
    [Table("vwCallLogReport")]
    public class CallLogReport
    {
        public int Id { get; set; }
        public string Channel { get; set; }
        public string Username { get; set; }
        public DateTime JoinTime { get; set; }
        public DateTime? LeaveTime { get; set; }
        public float TotalSeconds { get; set; }
        public TimeSpan TotalTime { get => TimeSpan.FromSeconds(this.TotalSeconds); }
        public int InCallBeforeJoined { get; set; }
        public int InCallAfterLeft { get; set; }
    }
}
