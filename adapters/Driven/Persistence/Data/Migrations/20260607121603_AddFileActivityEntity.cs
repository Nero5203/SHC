using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace adapters.Driven.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFileActivityEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentHash",
                table: "FileItems",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentHash",
                table: "FileItems");
        }
    }
}
