using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentMethod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcommerceUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CardholderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Last4Digits = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: false),
                    EncryptedCardNumber = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpirationMonth = table.Column<int>(type: "int", nullable: false),
                    ExpirationYear = table.Column<int>(type: "int", nullable: false),
                    CardBrand = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentMethod_EcommerceUser_EcommerceUserId",
                        column: x => x.EcommerceUserId,
                        principalTable: "EcommerceUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethod_EcommerceUserId",
                table: "PaymentMethod",
                column: "EcommerceUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentMethod");
        }
    }
}
