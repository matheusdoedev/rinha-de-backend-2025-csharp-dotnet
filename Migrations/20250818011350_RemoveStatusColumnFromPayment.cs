using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentBroker.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStatusColumnFromPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "payments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "payments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
