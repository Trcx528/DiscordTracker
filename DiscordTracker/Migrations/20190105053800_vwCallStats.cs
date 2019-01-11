using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordTracker.Migrations
{
    public partial class vwCallStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE VIEW CallStats AS
SELECT
	[User]
    ,coalesce(SUM(CASE WHEN EventType = 'Joined' THEN Duration ELSE 0 END) / 60.0, 0) as TimeInCall
	,coalesce(SUM(CASE WHEN EventType IN ('Admin Muted', 'Self Muted') THEN Duration ELSE 0 END) / 60.0, 0) as TimeMuted
	,coalesce(SUM(CASE WHEN EventType IN ('Admin Deafened', 'Self Deafened') THEN Duration ELSE 0 END) / 60.0, 0) as TimeDeafened
  FROM [dbo].[CallStatsDetails] csd
  WHERE csd.Start >= DATEADD(m, -1, GETDATE())
  GROUP BY [User]
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW CallStats");
        }
    }
}
