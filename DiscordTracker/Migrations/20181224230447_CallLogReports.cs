using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordTracker.Migrations
{
    public partial class CallLogReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE VIEW vwCallLogReport AS 
SElECT
cl.*
, Round((julianday(coalesce(cl.leavetime, current_timestamp)) - julianday(cl.jointime)) * 24 * 60 * 60) AS TotalSeconds
, (SELECT Count(jl.Id) FROM CallLogs jl WHERE cl.JoinTime >= jl.JoinTime AND cl.JoinTime < coalesce(jl.LeaveTime, current_timestamp) AND(Channel == cl.Channel AND Username <> cl.Username)) as InCallBeforeJoined
, (SELECT Count(ll.Id) FROM CallLogs ll WHERE coalesce(cl.LeaveTime, current_timestamp) >= ll.JoinTime AND cl.JoinTime < coalesce(ll.LeaveTime, current_timestamp) AND(Channel == cl.Channel AND Username <> cl.Username)) as InCallAfterLeft
 FROM CallLogs cl
 GROUP BY cl.Id, cl.Channel, cl.Username, cl.JoinTime, cl.LeaveTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW vwCallLogReport");
        }
    }
}
