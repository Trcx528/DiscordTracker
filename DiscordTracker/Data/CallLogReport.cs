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

        /*
         * CREATE VIEW vwCallLogReport AS 
SElECT 
cl.*
, Round((julianday(cl.leavetime) - julianday(cl.jointime)) * 24 * 60 *60) AS TotalSeconds
, (SELECT Count(jl.Id) FROM CallLogs jl WHERE cl.JoinTime >= jl.JoinTime AND cl.JoinTime < jl.LeaveTime AND (Channel == cl.Channel AND Username <> cl.Username)) as InCallBeforeJoined
, (SELECT Count(ll.Id) FROM CallLogs ll WHERE cl.LeaveTime >= ll.JoinTime AND cl.JoinTime < ll.LeaveTime AND (Channel == cl.Channel AND Username <> cl.Username)) as InCallAfterLeft
 FROM CallLogs cl
 GROUP BY cl.Id, cl.Channel, cl.Username, cl.JoinTime, cl.LeaveTime
         */
    }
}
