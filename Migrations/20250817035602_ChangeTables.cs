using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentBroker.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessingMethod",
                table: "payment-processing");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "payment-processing");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "payments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Method",
                table: "payment-processing",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "payment-processing");

            migrationBuilder.AddColumn<int>(
                name: "ProcessingMethod",
                table: "payment-processing",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "payment-processing",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
