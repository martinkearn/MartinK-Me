using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MartinKRC2.Data.Migrations
{
    public partial class ResourceResourceGroupMany2Many : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resource_ResourceGroup_ResourceGroupId",
                table: "Resource");

            migrationBuilder.DropIndex(
                name: "IX_Resource_ResourceGroupId",
                table: "Resource");

            migrationBuilder.DropColumn(
                name: "ResourceGroupId",
                table: "Resource");

            migrationBuilder.CreateTable(
                name: "ResourceResourceGroup",
                columns: table => new
                {
                    ResourceGroupId = table.Column<int>(nullable: false),
                    ResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceResourceGroup", x => new { x.ResourceGroupId, x.ResourceId });
                    table.ForeignKey(
                        name: "FK_ResourceResourceGroup_ResourceGroup_ResourceGroupId",
                        column: x => x.ResourceGroupId,
                        principalTable: "ResourceGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceResourceGroup_Resource_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "Resource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceResourceGroup_ResourceGroupId",
                table: "ResourceResourceGroup",
                column: "ResourceGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceResourceGroup_ResourceId",
                table: "ResourceResourceGroup",
                column: "ResourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceResourceGroup");

            migrationBuilder.AddColumn<int>(
                name: "ResourceGroupId",
                table: "Resource",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Resource_ResourceGroupId",
                table: "Resource",
                column: "ResourceGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Resource_ResourceGroup_ResourceGroupId",
                table: "Resource",
                column: "ResourceGroupId",
                principalTable: "ResourceGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
