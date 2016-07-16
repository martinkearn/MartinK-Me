using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MartinKRC2.Data.Migrations
{
    public partial class ResourceTalkMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResourceTalk",
                columns: table => new
                {
                    TalkId = table.Column<int>(nullable: false),
                    ResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceTalk", x => new { x.TalkId, x.ResourceId });
                    table.ForeignKey(
                        name: "FK_ResourceTalk_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceTalk_Talk_TalkId",
                        column: x => x.TalkId,
                        principalTable: "Talk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceTalk_ResourceId",
                table: "ResourceTalk",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceTalk_TalkId",
                table: "ResourceTalk",
                column: "TalkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceTalk");
        }
    }
}
