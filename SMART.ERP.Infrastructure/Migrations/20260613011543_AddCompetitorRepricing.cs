using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompetitorRepricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompetitorSource",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CompetitorName = table.Column<string>(type: "varchar(50)", nullable: false),
                    ProductUrl = table.Column<string>(type: "varchar(max)", nullable: false),
                    ParseStrategy = table.Column<int>(type: "int", nullable: false),
                    PriceSelector = table.Column<string>(type: "varchar(300)", nullable: true),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TaxBasis = table.Column<int>(type: "int", nullable: false),
                    Currency = table.Column<string>(type: "varchar(3)", nullable: false),
                    LastCheckedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastObservedPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    LastObservedInStock = table.Column<bool>(type: "bit", nullable: true),
                    LastError = table.Column<string>(type: "varchar(500)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetitorSource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompetitorSource_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PriceChangeLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CompetitorSourceIdMin = table.Column<int>(type: "int", nullable: true),
                    MinCompetitorPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    OldPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ProposedPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AppliedPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    FloorHit = table.Column<bool>(type: "bit", nullable: false),
                    Applied = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "varchar(500)", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppliedUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AppliedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceChangeLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceChangeLog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RepricingRule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UndercutMode = table.Column<int>(type: "int", nullable: false),
                    UndercutValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MinMarginPercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    RoundingMode = table.Column<int>(type: "int", nullable: false),
                    MaxDecreasePercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    MinChangeThreshold = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AutoApply = table.Column<bool>(type: "bit", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepricingRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepricingRule_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RepricingSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonitoringEnabled = table.Column<bool>(type: "bit", nullable: false),
                    GlobalAutoApply = table.Column<bool>(type: "bit", nullable: false),
                    DefaultUndercutMode = table.Column<int>(type: "int", nullable: false),
                    DefaultUndercutValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    DefaultMinMarginPercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    DefaultRoundingMode = table.Column<int>(type: "int", nullable: false),
                    DefaultMaxDecreasePercent = table.Column<decimal>(type: "decimal(9,4)", precision: 9, scale: 4, nullable: false),
                    DefaultMinChangeThreshold = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepricingSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompetitorSource_ProductId_CompetitorName",
                table: "CompetitorSource",
                columns: new[] { "ProductId", "CompetitorName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceChangeLog_ProductId_CreatedUtc",
                table: "PriceChangeLog",
                columns: new[] { "ProductId", "CreatedUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_RepricingRule_ProductId",
                table: "RepricingRule",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompetitorSource");

            migrationBuilder.DropTable(
                name: "PriceChangeLog");

            migrationBuilder.DropTable(
                name: "RepricingRule");

            migrationBuilder.DropTable(
                name: "RepricingSettings");
        }
    }
}
