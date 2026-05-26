using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditCardPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CreditCardPayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    CreditCardInternalBankAccountId = table.Column<int>(type: "int", nullable: false),
                    SourceInternalBankAccountId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCardPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CreditCardPayment_InternalBankAccount_CreditCardInternalBankAccountId",
                        column: x => x.CreditCardInternalBankAccountId,
                        principalTable: "InternalBankAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CreditCardPayment_InternalBankAccount_SourceInternalBankAccountId",
                        column: x => x.SourceInternalBankAccountId,
                        principalTable: "InternalBankAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_Code",
                table: "CreditCardPayment",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_CreditCardInternalBankAccountId",
                table: "CreditCardPayment",
                column: "CreditCardInternalBankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_Date",
                table: "CreditCardPayment",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_CreditCardPayment_SourceInternalBankAccountId",
                table: "CreditCardPayment",
                column: "SourceInternalBankAccountId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditCardPayment");
        }
    }
}
