using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessSolution.Migrations
{
    /// <inheritdoc />
    public partial class newtableSupplierTransactionHistoryadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SupplierTransactionHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false),
                    SupplierOrderId = table.Column<long>(type: "bigint", nullable: true),
                    TransactionNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Payment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SupplierPreviousBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SupplierNewBalance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedBy = table.Column<long>(type: "bigint", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierTransactionHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierTransactionHistory_SupplierOrders_SupplierOrderId",
                        column: x => x.SupplierOrderId,
                        principalTable: "SupplierOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SupplierTransactionHistory_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierTransactionHistory_SupplierId",
                table: "SupplierTransactionHistory",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierTransactionHistory_SupplierOrderId",
                table: "SupplierTransactionHistory",
                column: "SupplierOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierTransactionHistory");
        }
    }
}
