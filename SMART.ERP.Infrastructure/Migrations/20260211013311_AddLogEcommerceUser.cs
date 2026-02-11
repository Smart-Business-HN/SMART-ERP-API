using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLogEcommerceUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogEcommerceUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EcommerceUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEcommerceUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogEcommerceUser_EcommerceUser_EcommerceUserId",
                        column: x => x.EcommerceUserId,
                        principalTable: "EcommerceUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogEcommerceUser_EcommerceUserId",
                table: "LogEcommerceUser",
                column: "EcommerceUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogEcommerceUser");
        }
    }
}
