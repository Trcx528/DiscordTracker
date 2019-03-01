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
CROSS APPLY
	(SELECT TOP 1 
		*
	FROM VoiceEventLog ve 
	WHERE ve.Date > vel.Date
		AND ve.ChannelId = vel.ChannelId
		AND ve.UserId = vel.UserId
		AND ve.EventType IN (SELECT SecondaryEvent FROM RelatedVoiceEvents WHERE InitalEvent = vel.EventType)
	ORDER BY Date
	) vet
WHERE vel.EventType IN (SELECT InitalEvent FROM RelatedVoiceEvents Where IsPrimary = 1) 
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW CallStatsDetails");
        }
    }
}
