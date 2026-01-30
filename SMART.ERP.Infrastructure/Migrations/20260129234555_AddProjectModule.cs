using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Quotation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "PurchaseBill",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "NonBillableExpense",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Invoice",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ProjectCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExecutionBudget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    PrefixId = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Projects_Prefix_PrefixId",
                        column: x => x.PrefixId,
                        principalTable: "Prefix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Projects_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_ProjectId",
                table: "Quotation",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBill_ProjectId",
                table: "PurchaseBill",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_NonBillableExpense_ProjectId",
                table: "NonBillableExpense",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_ProjectId",
                table: "Invoice",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CustomerId",
                table: "Projects",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_PrefixId",
                table: "Projects",
                column: "PrefixId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_StatusId",
                table: "Projects",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Projects_ProjectId",
                table: "Invoice",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NonBillableExpense_Projects_ProjectId",
                table: "NonBillableExpense",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseBill_Projects_ProjectId",
                table: "PurchaseBill",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotation_Projects_ProjectId",
                table: "Quotation",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_Projects_ProjectId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_NonBillableExpense_Projects_ProjectId",
                table: "NonBillableExpense");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseBill_Projects_ProjectId",
                table: "PurchaseBill");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotation_Projects_ProjectId",
                table: "Quotation");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Quotation_ProjectId",
                table: "Quotation");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseBill_ProjectId",
                table: "PurchaseBill");

            migrationBuilder.DropIndex(
                name: "IX_NonBillableExpense_ProjectId",
                table: "NonBillableExpense");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_ProjectId",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Quotation");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "PurchaseBill");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "NonBillableExpense");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Invoice");
        }
    }
}
