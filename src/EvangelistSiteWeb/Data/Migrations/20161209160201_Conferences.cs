using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EvangelistSiteWeb.Data.Migrations
{
    public partial class Conferences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conference",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ConferenceTitle = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    ExternalUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conference", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConferenceTalk",
                columns: table => new
                {
                    TalkId = table.Column<int>(nullable: false),
                    ConferenceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConferenceTalk", x => new { x.TalkId, x.ConferenceId });
                    table.ForeignKey(
                        name: "FK_ConferenceTalk_Conference_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "Conference",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConferenceTalk_Talk_TalkId",
                        column: x => x.TalkId,
                        principalTable: "Talk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceTalk_ConferenceId",
                table: "ConferenceTalk",
                column: "ConferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceTalk_TalkId",
                table: "ConferenceTalk",
                column: "TalkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConferenceTalk");

            migrationBuilder.DropTable(
                name: "Conference");
        }
    }
}
