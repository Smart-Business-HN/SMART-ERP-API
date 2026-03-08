using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringInvoiceFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecurringInvoiceTemplateId",
                table: "Invoice",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecurringInvoiceTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchOfficeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoicePaymentTypeId = table.Column<int>(type: "int", nullable: false),
                    DayOfMonth = table.Column<int>(type: "int", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastGeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NextGenerationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProjectId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringInvoiceTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringInvoiceTemplate_BranchOffice_BranchOfficeId",
                        column: x => x.BranchOfficeId,
                        principalTable: "BranchOffice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringInvoiceTemplate_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringInvoiceTemplate_InvoicePaymentType_InvoicePaymentTypeId",
                        column: x => x.InvoicePaymentTypeId,
                        principalTable: "InvoicePaymentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringInvoiceTemplate_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringInvoiceTemplate_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringInvoiceTemplate_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecurringInvoiceLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecurringInvoiceTemplateId = table.Column<int>(type: "int", nullable: false),
                    GeneratedInvoiceId = table.Column<int>(type: "int", nullable: true),
                    ExecutionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Succeeded = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringInvoiceLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringInvoiceLog_Invoice_GeneratedInvoiceId",
                        column: x => x.GeneratedInvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringInvoiceLog_RecurringInvoiceTemplate_RecurringInvoiceTemplateId",
                        column: x => x.RecurringInvoiceTemplateId,
                        principalTable: "RecurringInvoiceTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecurringInvoiceTemplateItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecurringInvoiceTemplateId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ProductDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringInvoiceTemplateItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringInvoiceTemplateItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringInvoiceTemplateItem_RecurringInvoiceTemplate_RecurringInvoiceTemplateId",
                        column: x => x.RecurringInvoiceTemplateId,
                        principalTable: "RecurringInvoiceTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecurringInvoiceTemplateItem_Tax_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Tax",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_RecurringInvoiceTemplateId",
                table: "Invoice",
                column: "RecurringInvoiceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringInvoiceLog_GeneratedInvoiceId",
                table: "RecurringInvoiceLog",
                column: "GeneratedInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringInvoiceLog_RecurringInvoiceTemplateId",
                table: "RecurringInvoiceLog",
                column: "RecurringInvoiceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringInvoiceTemplate_BranchOfficeId",
                table: "RecurringInvoiceTemplate",
                column: "BranchOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringInvoiceTemplate_CustomerId",
                table: "RecurringInvoiceTemplate",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringInvoiceTemplate_InvoicePaymentTypeId",
                table: "RecurringInvoiceTemplate",
                column: "InvoicePaymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringInvoiceTemplate_ProjectId",
                table: "RecurringInvoiceTemplate",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringInvoiceTemplate_StatusId",
                table: "RecurringInvoiceTemplate",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringInvoiceTemplate_UserId",
                table: "RecurringInvoiceTemplate",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringInvoiceTemplateItem_ProductId",
                table: "RecurringInvoiceTemplateItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringInvoiceTemplateItem_RecurringInvoiceTemplateId",
                table: "RecurringInvoiceTemplateItem",
                column: "RecurringInvoiceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringInvoiceTemplateItem_TaxId",
                table: "RecurringInvoiceTemplateItem",
                column: "TaxId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_RecurringInvoiceTemplate_RecurringInvoiceTemplateId",
                table: "Invoice",
                column: "RecurringInvoiceTemplateId",
                principalTable: "RecurringInvoiceTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_RecurringInvoiceTemplate_RecurringInvoiceTemplateId",
                table: "Invoice");

            migrationBuilder.DropTable(
                name: "RecurringInvoiceLog");

            migrationBuilder.DropTable(
                name: "RecurringInvoiceTemplateItem");

            migrationBuilder.DropTable(
                name: "RecurringInvoiceTemplate");

            migrationBuilder.DropIndex(
                name: "IX_Invoice_RecurringInvoiceTemplateId",
                table: "Invoice");

            migrationBuilder.DropColumn(
                name: "RecurringInvoiceTemplateId",
                table: "Invoice");
        }
    }
}
