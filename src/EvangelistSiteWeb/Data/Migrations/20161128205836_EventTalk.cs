using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EvangelistSiteWeb.Data.Migrations
{
    public partial class EventTalk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalUrl",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "Talk",
                table: "Event");

            migrationBuilder.CreateTable(
                name: "EventTalk",
                columns: table => new
                {
                    TalkId = table.Column<int>(nullable: false),
                    EventId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventTalk", x => new { x.TalkId, x.EventId });
                    table.ForeignKey(
                        name: "FK_EventTalk_Event_EventId",
                        column: x => x.EventId,
                        principalTable: "Event",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventTalk_Talk_TalkId",
                        column: x => x.TalkId,
                        principalTable: "Talk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventTalk_EventId",
                table: "EventTalk",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventTalk_TalkId",
                table: "EventTalk",
                column: "TalkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventTalk");

            migrationBuilder.AddColumn<string>(
                name: "InternalUrl",
                table: "Event",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Talk",
                table: "Event",
                nullable: true);
        }
    }
}
