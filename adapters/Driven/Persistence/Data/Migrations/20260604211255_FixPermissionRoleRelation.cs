using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace adapters.Driven.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixPermissionRoleRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Roles_RoleId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Roles_RoleId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_RoleId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_RoleId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "AuditLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "Permissions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "AuditLogs",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_RoleId",
                table: "Permissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_RoleId",
                table: "AuditLogs",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Roles_RoleId",
                table: "AuditLogs",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Roles_RoleId",
                table: "Permissions",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "RoleId");
        }
    }
}
