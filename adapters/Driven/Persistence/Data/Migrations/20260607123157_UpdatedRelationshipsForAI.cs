using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace adapters.Driven.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRelationshipsForAI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIFileInsight_FileItems_FileItemId",
                table: "AIFileInsight");

            migrationBuilder.DropForeignKey(
                name: "FK_AISuggestion_FileItems_FileItemId",
                table: "AISuggestion");

            migrationBuilder.DropForeignKey(
                name: "FK_AISuggestion_Users_UserId",
                table: "AISuggestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AISuggestion",
                table: "AISuggestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AIFileInsight",
                table: "AIFileInsight");

            migrationBuilder.RenameTable(
                name: "AISuggestion",
                newName: "AISuggestions");

            migrationBuilder.RenameTable(
                name: "AIFileInsight",
                newName: "AIFileInsights");

            migrationBuilder.RenameIndex(
                name: "IX_AISuggestion_UserId",
                table: "AISuggestions",
                newName: "IX_AISuggestions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AISuggestion_FileItemId",
                table: "AISuggestions",
                newName: "IX_AISuggestions_FileItemId");

            migrationBuilder.RenameIndex(
                name: "IX_AIFileInsight_FileItemId",
                table: "AIFileInsights",
                newName: "IX_AIFileInsights_FileItemId");

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
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AISuggestions",
                table: "AISuggestions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AIFileInsights",
                table: "AIFileInsights",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "FileActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    FileItemId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FileItemId1 = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    UserId1 = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileActivities_FileItems_FileItemId",
                        column: x => x.FileItemId,
                        principalTable: "FileItems",
                        principalColumn: "FileItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileActivities_FileItems_FileItemId1",
                        column: x => x.FileItemId1,
                        principalTable: "FileItems",
                        principalColumn: "FileItemId");
                    table.ForeignKey(
                        name: "FK_FileActivities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileActivities_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "UserId");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AISuggestions_FileItemId1",
                table: "AISuggestions",
                column: "FileItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_AISuggestions_UserId1",
                table: "AISuggestions",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_FileActivities_FileItemId",
                table: "FileActivities",
                column: "FileItemId");

            migrationBuilder.CreateIndex(
                name: "IX_FileActivities_FileItemId1",
                table: "FileActivities",
                column: "FileItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_FileActivities_UserId",
                table: "FileActivities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FileActivities_UserId1",
                table: "FileActivities",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AIFileInsights_FileItems_FileItemId",
                table: "AIFileInsights",
                column: "FileItemId",
                principalTable: "FileItems",
                principalColumn: "FileItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AISuggestions_FileItems_FileItemId",
                table: "AISuggestions",
                column: "FileItemId",
                principalTable: "FileItems",
                principalColumn: "FileItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AISuggestions_FileItems_FileItemId1",
                table: "AISuggestions",
                column: "FileItemId1",
                principalTable: "FileItems",
                principalColumn: "FileItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_AISuggestions_Users_UserId",
                table: "AISuggestions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AISuggestions_Users_UserId1",
                table: "AISuggestions",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AIFileInsights_FileItems_FileItemId",
                table: "AIFileInsights");

            migrationBuilder.DropForeignKey(
                name: "FK_AISuggestions_FileItems_FileItemId",
                table: "AISuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_AISuggestions_FileItems_FileItemId1",
                table: "AISuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_AISuggestions_Users_UserId",
                table: "AISuggestions");

            migrationBuilder.DropForeignKey(
                name: "FK_AISuggestions_Users_UserId1",
                table: "AISuggestions");

            migrationBuilder.DropTable(
                name: "FileActivities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AISuggestions",
                table: "AISuggestions");

            migrationBuilder.DropIndex(
                name: "IX_AISuggestions_FileItemId1",
                table: "AISuggestions");

            migrationBuilder.DropIndex(
                name: "IX_AISuggestions_UserId1",
                table: "AISuggestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AIFileInsights",
                table: "AIFileInsights");

            migrationBuilder.DropColumn(
                name: "FileItemId1",
                table: "AISuggestions");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "AISuggestions");

            migrationBuilder.RenameTable(
                name: "AISuggestions",
                newName: "AISuggestion");

            migrationBuilder.RenameTable(
                name: "AIFileInsights",
                newName: "AIFileInsight");

            migrationBuilder.RenameIndex(
                name: "IX_AISuggestions_UserId",
                table: "AISuggestion",
                newName: "IX_AISuggestion_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AISuggestions_FileItemId",
                table: "AISuggestion",
                newName: "IX_AISuggestion_FileItemId");

            migrationBuilder.RenameIndex(
                name: "IX_AIFileInsights_FileItemId",
                table: "AIFileInsight",
                newName: "IX_AIFileInsight_FileItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AISuggestion",
                table: "AISuggestion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AIFileInsight",
                table: "AIFileInsight",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AIFileInsight_FileItems_FileItemId",
                table: "AIFileInsight",
                column: "FileItemId",
                principalTable: "FileItems",
                principalColumn: "FileItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AISuggestion_FileItems_FileItemId",
                table: "AISuggestion",
                column: "FileItemId",
                principalTable: "FileItems",
                principalColumn: "FileItemId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AISuggestion_Users_UserId",
                table: "AISuggestion",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
