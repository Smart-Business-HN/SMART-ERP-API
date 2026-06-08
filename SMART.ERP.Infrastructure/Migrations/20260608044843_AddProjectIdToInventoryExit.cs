using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectIdToInventoryExit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "InventoryExit",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryExit_ProjectId",
                table: "InventoryExit",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryExit_Projects_ProjectId",
                table: "InventoryExit",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryExit_Projects_ProjectId",
                table: "InventoryExit");

            migrationBuilder.DropIndex(
                name: "IX_InventoryExit_ProjectId",
                table: "InventoryExit");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "InventoryExit");
        }
    }
}
