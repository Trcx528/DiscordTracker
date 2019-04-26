using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordTracker.Migrations
{
    public partial class ReactionTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscordChannels",
                columns: table => new
                {
                    Id = table.Column<decimal>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscordMessages",
                columns: table => new
                {
                    Id = table.Column<decimal>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    AuthorId = table.Column<decimal>(nullable: false),
                    ChannelId = table.Column<decimal>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscordMessages_DiscordUser_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "DiscordUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscordMessages_DiscordChannels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "DiscordChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserMessageReactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReactorId = table.Column<decimal>(nullable: false),
                    Reaction = table.Column<string>(nullable: true),
                    MessageId = table.Column<decimal>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessageReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMessageReactions_DiscordMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "DiscordMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscordMessages_AuthorId",
                table: "DiscordMessages",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordMessages_ChannelId",
                table: "DiscordMessages",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMessageReactions_MessageId",
                table: "UserMessageReactions",
                column: "MessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMessageReactions");

            migrationBuilder.DropTable(
                name: "DiscordMessages");

            migrationBuilder.DropTable(
                name: "DiscordChannels");
        }
    }
}
