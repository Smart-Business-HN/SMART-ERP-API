using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthlySaleDeclaration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonthlySaleDeclaration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Period = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    TotalInvoices = table.Column<int>(type: "int", nullable: false),
                    Exempt = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Exonerated = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxedAt15Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxedAt18Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes15Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes18Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalTaxes = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlySaleDeclaration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlySaleDeclaration_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeclaredSaleInvoice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonthlySaleDeclarationId = table.Column<int>(type: "int", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    CustomerRTN = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    InvoiceDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Cai = table.Column<string>(type: "nvarchar(37)", maxLength: 37, nullable: false),
                    Establishment = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    EmissionPoint = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    KindOfDocument = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Correlative = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    SaleWithExoneration = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Exempt = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Exonerated = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxedAt15Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxedAt18Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes15Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes18Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclaredSaleInvoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeclaredSaleInvoice_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeclaredSaleInvoice_MonthlySaleDeclaration_MonthlySaleDeclarationId",
                        column: x => x.MonthlySaleDeclarationId,
                        principalTable: "MonthlySaleDeclaration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeclaredSaleInvoice_InvoiceId",
                table: "DeclaredSaleInvoice",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeclaredSaleInvoice_MonthlySaleDeclarationId",
                table: "DeclaredSaleInvoice",
                column: "MonthlySaleDeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlySaleDeclaration_StatusId",
                table: "MonthlySaleDeclaration",
                column: "StatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeclaredSaleInvoice");

            migrationBuilder.DropTable(
                name: "MonthlySaleDeclaration");
        }
    }
}
