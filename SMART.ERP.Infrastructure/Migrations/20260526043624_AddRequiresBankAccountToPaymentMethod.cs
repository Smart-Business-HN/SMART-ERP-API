using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRequiresBankAccountToPaymentMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresBankAccount",
                table: "TypeOfPaymentMethod",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"
                UPDATE TypeOfPaymentMethod
                SET RequiresBankAccount = 1
                WHERE Name LIKE N'%Transferencia%'
                   OR Name LIKE N'%Cheque%'
                   OR Name LIKE N'%Tarjeta%'
                   OR Name LIKE N'%Dep[oó]sito%'
                   OR Name LIKE N'%Transfer%';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiresBankAccount",
                table: "TypeOfPaymentMethod");
        }
    }
}
