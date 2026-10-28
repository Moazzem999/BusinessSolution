using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessSolution.Migrations
{
    /// <inheritdoc />
    public partial class newtableSupplierPaymentsadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SupplierPaymentId",
                table: "SupplierTransactionHistory",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SupplierPayments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Payment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierPayments_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierTransactionHistory_SupplierPaymentId",
                table: "SupplierTransactionHistory",
                column: "SupplierPaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPayments_SupplierId",
                table: "SupplierPayments",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierTransactionHistory_SupplierPayments_SupplierPaymentId",
                table: "SupplierTransactionHistory",
                column: "SupplierPaymentId",
                principalTable: "SupplierPayments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierTransactionHistory_SupplierPayments_SupplierPaymentId",
                table: "SupplierTransactionHistory");

            migrationBuilder.DropTable(
                name: "SupplierPayments");

            migrationBuilder.DropIndex(
                name: "IX_SupplierTransactionHistory_SupplierPaymentId",
                table: "SupplierTransactionHistory");

            migrationBuilder.DropColumn(
                name: "SupplierPaymentId",
                table: "SupplierTransactionHistory");
        }
    }
}
