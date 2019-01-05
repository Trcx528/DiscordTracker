using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordTracker.Migrations
{
    public partial class vwCallStatsDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
Create View [dbo].[CallStatsDetails] AS
SELECT
	vel.Date AS Start
	,vet.Date AS [End]
	,DateDiff(MINUTE, vel.Date , vet.Date) as Duration
	, dvc.Name as Channel
	, du.IRLName as [User]
	, vel.EventType
FROM VoiceEventLog vel
JOIN RelatedVoiceEvents rve
	ON vel.EventType = rve.InitalEvent
JOIN DiscordUser du
	ON du.Id = vel.UserId
JOIN DiscordVoiceChannel dvc
	ON dvc.Id = vel.ChannelId
LEFT JOIN VoiceEventLog vet
	ON vel.ChannelId = vet.ChannelId 
	AND vel.UserId = vet.UserId 
	AND rve.SecondaryEvent = vet.EventType 
	AND vet.Date > vel.Date 
	AND DATEDIFF(HOUR, vel.date, vet.date) < 24
WHERE vel.EventType IN (SELECT InitalEvent FROM RelatedVoiceEvents) 
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW CallStatsDetails");
        }
    }
}
