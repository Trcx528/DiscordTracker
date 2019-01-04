using DiscordTracker.Data;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordTracker.Migrations
{
    public partial class InsertRelatedVoiceEvents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var ctx = new ApplicationDataContext();
            ctx.Add(new RelatedVoiceEvent() { InitalEvent = "Joined", SecondaryEvent = "Left" });
            ctx.Add(new RelatedVoiceEvent() { InitalEvent = "Admin Deafened", SecondaryEvent = "Admin Undeafened" });
            ctx.Add(new RelatedVoiceEvent() { InitalEvent = "Admin Muted", SecondaryEvent = "Admin Unmuted" });
            ctx.Add(new RelatedVoiceEvent() { InitalEvent = "Self Deafened", SecondaryEvent = "Self Undeafened" });
            ctx.Add(new RelatedVoiceEvent() { InitalEvent = "Self Muted", SecondaryEvent = "Self Unmuted" });
            ctx.SaveChanges();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
