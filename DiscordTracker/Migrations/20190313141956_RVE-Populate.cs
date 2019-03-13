using DiscordTracker.Data;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordTracker.Migrations
{
    public partial class RVEPopulate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var ctx = new ApplicationDataContext();
            ctx.Add(new RelatedVoiceEvent() { InitalEvent = "Joined", SecondaryEvent = "Left", IsPrimary = true });
            ctx.Add(new RelatedVoiceEvent() { InitalEvent = "Admin Deafened", SecondaryEvent = "Admin Undeafened", IsPrimary = true });
            ctx.Add(new RelatedVoiceEvent() { InitalEvent = "Admin Muted", SecondaryEvent = "Admin Unmuted", IsPrimary = true });
            ctx.Add(new RelatedVoiceEvent() { InitalEvent = "Self Deafened", SecondaryEvent = "Self Undeafened", IsPrimary = true });
            ctx.Add(new RelatedVoiceEvent() { InitalEvent = "Self Muted", SecondaryEvent = "Self Unmuted", IsPrimary = true });
            ctx.Add(new RelatedVoiceEvent() { InitalEvent = "Self Muted", SecondaryEvent = "Left", IsPrimary = false });
            ctx.Add(new RelatedVoiceEvent() { InitalEvent = "Self Deafened", SecondaryEvent = "Left", IsPrimary = false });
            ctx.Add(new RelatedVoiceEvent() { InitalEvent = "Admin Deafened", SecondaryEvent = "Left", IsPrimary = false });
            ctx.Add(new RelatedVoiceEvent() { InitalEvent = "Admin Muted", SecondaryEvent = "Left", IsPrimary = false });
            ctx.SaveChanges();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
