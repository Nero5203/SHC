using System;
using adapters.Driven.Persistence.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace adapters.Driven.Persistence.Data.Migrations
{
    [DbContext(typeof(ShcDbContext))]
    [Migration("20260607170000_CleanupAIShadowForeignKeys")]
    public partial class CleanupAIShadowForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AISuggestions_FileItems_FileItemId1",
                table: "AISuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_AISuggestions_Users_UserId1",
                table: "AISuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_FileActivities_FileItems_FileItemId1",
                table: "FileActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_FileActivities_Users_UserId1",
                table: "FileActivities");

            migrationBuilder.DropIndex(
                name: "IX_AISuggestions_FileItemId1",
                table: "AISuggestions");

            migrationBuilder.DropIndex(
                name: "IX_AISuggestions_UserId1",
                table: "AISuggestions");

            migrationBuilder.DropIndex(
                name: "IX_FileActivities_FileItemId1",
                table: "FileActivities");

            migrationBuilder.DropIndex(
                name: "IX_FileActivities_UserId1",
                table: "FileActivities");

            migrationBuilder.DropColumn(
                name: "FileItemId1",
                table: "AISuggestions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "AISuggestions");

            migrationBuilder.DropColumn(
                name: "FileItemId1",
                table: "FileActivities");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "FileActivities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FileItemId1",
                table: "AISuggestions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "AISuggestions",
                type: "char(36)",
                nullable: false,
                defaultValue: Guid.Empty,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "FileItemId1",
                table: "FileActivities",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "FileActivities",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_AISuggestions_FileItemId1",
                table: "AISuggestions",
                column: "FileItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_AISuggestions_UserId1",
                table: "AISuggestions",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_FileActivities_FileItemId1",
                table: "FileActivities",
                column: "FileItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_FileActivities_UserId1",
                table: "FileActivities",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AISuggestions_FileItems_FileItemId1",
                table: "AISuggestions",
                column: "FileItemId1",
                principalTable: "FileItems",
                principalColumn: "FileItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_AISuggestions_Users_UserId1",
                table: "AISuggestions",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileActivities_FileItems_FileItemId1",
                table: "FileActivities",
                column: "FileItemId1",
                principalTable: "FileItems",
                principalColumn: "FileItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileActivities_Users_UserId1",
                table: "FileActivities",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
