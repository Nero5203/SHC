using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace adapters.Driven.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStripePaymentMetadataToPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProviderCheckoutSessionId",
                table: "Purchases",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ProviderPaymentIntentId",
                table: "Purchases",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderCheckoutSessionId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "ProviderPaymentIntentId",
                table: "Purchases");
        }
    }
}
