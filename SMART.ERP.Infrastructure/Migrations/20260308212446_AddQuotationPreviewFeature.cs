using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuotationPreviewFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccessToken",
                table: "Quotation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AccessTokenGeneratedDate",
                table: "Quotation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QuotationComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuotationId = table.Column<int>(type: "int", nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AuthorEmail = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Message = table.Column<string>(type: "varchar(600)", nullable: false),
                    IsFromClient = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotationComment_Quotation_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationComment_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuotationItemObservation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductOfferedId = table.Column<int>(type: "int", nullable: false),
                    QuotationId = table.Column<int>(type: "int", nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Observation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationItemObservation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotationItemObservation_ProductOffered_ProductOfferedId",
                        column: x => x.ProductOfferedId,
                        principalTable: "ProductOffered",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationItemObservation_Quotation_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_AccessToken",
                table: "Quotation",
                column: "AccessToken",
                unique: true,
                filter: "[AccessToken] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationComment_QuotationId",
                table: "QuotationComment",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationComment_UserId",
                table: "QuotationComment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItemObservation_ProductOfferedId",
                table: "QuotationItemObservation",
                column: "ProductOfferedId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItemObservation_QuotationId",
                table: "QuotationItemObservation",
                column: "QuotationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuotationComment");

            migrationBuilder.DropTable(
                name: "QuotationItemObservation");

            migrationBuilder.DropIndex(
                name: "IX_Quotation_AccessToken",
                table: "Quotation");

            migrationBuilder.DropColumn(
                name: "AccessToken",
                table: "Quotation");

            migrationBuilder.DropColumn(
                name: "AccessTokenGeneratedDate",
                table: "Quotation");
        }
    }
}
