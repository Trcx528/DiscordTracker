using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordTracker.Migrations
{
    public partial class UpdateUserIRLName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE DiscordUser SET IRLName = Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
