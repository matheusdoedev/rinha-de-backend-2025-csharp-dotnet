using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentBroker.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentProcessingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payment-processing",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ProcessingMethod = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PaymentId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment-processing", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payment-processing_payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_payment-processing_PaymentId",
                table: "payment-processing",
                column: "PaymentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payment-processing");
        }
    }
}
