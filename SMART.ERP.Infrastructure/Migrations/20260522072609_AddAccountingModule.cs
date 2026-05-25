using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountingModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LedgerAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    NormalBalanceSide = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    IsPostable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsSystem = table.Column<bool>(type: "bit", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ExpenseAccountId = table.Column<int>(type: "int", nullable: true),
                    IncomeAccountId = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LedgerAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LedgerAccount_ExpenseAccount_ExpenseAccountId",
                        column: x => x.ExpenseAccountId,
                        principalTable: "ExpenseAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LedgerAccount_IncomeAccount_IncomeAccountId",
                        column: x => x.IncomeAccountId,
                        principalTable: "IncomeAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LedgerAccount_LedgerAccount_ParentId",
                        column: x => x.ParentId,
                        principalTable: "LedgerAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FiscalPeriod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FiscalYearId = table.Column<int>(type: "int", nullable: false),
                    PeriodNumber = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ClosedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalPeriod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FiscalPeriodId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Reference = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Source = table.Column<int>(type: "int", nullable: false),
                    SourceDocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SourceDocumentId = table.Column<int>(type: "int", nullable: true),
                    TotalDebit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalCredit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PostedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PostedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    ReversesJournalEntryId = table.Column<int>(type: "int", nullable: true),
                    ReversedByJournalEntryId = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalEntry_FiscalPeriod_FiscalPeriodId",
                        column: x => x.FiscalPeriodId,
                        principalTable: "FiscalPeriod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalEntry_JournalEntry_ReversedByJournalEntryId",
                        column: x => x.ReversedByJournalEntryId,
                        principalTable: "JournalEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalEntry_JournalEntry_ReversesJournalEntryId",
                        column: x => x.ReversesJournalEntryId,
                        principalTable: "JournalEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FiscalYear",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    ClosedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    ClosingJournalEntryId = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalYear", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FiscalYear_JournalEntry_ClosingJournalEntryId",
                        column: x => x.ClosingJournalEntryId,
                        principalTable: "JournalEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JournalEntryLine",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalEntryId = table.Column<int>(type: "int", nullable: false),
                    LedgerAccountId = table.Column<int>(type: "int", nullable: false),
                    LineNumber = table.Column<int>(type: "int", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ProjectId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntryLine", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalEntryLine_JournalEntry_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JournalEntryLine_LedgerAccount_LedgerAccountId",
                        column: x => x.LedgerAccountId,
                        principalTable: "LedgerAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalEntryLine_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FiscalPeriod_FiscalYearId_PeriodNumber",
                table: "FiscalPeriod",
                columns: new[] { "FiscalYearId", "PeriodNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FiscalYear_ClosingJournalEntryId",
                table: "FiscalYear",
                column: "ClosingJournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_FiscalYear_Year",
                table: "FiscalYear",
                column: "Year",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_EntryDate",
                table: "JournalEntry",
                column: "EntryDate");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_EntryNumber",
                table: "JournalEntry",
                column: "EntryNumber",
                unique: true,
                filter: "[EntryNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_FiscalPeriodId",
                table: "JournalEntry",
                column: "FiscalPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_ReversedByJournalEntryId",
                table: "JournalEntry",
                column: "ReversedByJournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_ReversesJournalEntryId",
                table: "JournalEntry",
                column: "ReversesJournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntry_Status",
                table: "JournalEntry",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLine_JournalEntryId",
                table: "JournalEntryLine",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLine_LedgerAccountId",
                table: "JournalEntryLine",
                column: "LedgerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLine_ProjectId",
                table: "JournalEntryLine",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerAccount_Code",
                table: "LedgerAccount",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LedgerAccount_ExpenseAccountId",
                table: "LedgerAccount",
                column: "ExpenseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerAccount_IncomeAccountId",
                table: "LedgerAccount",
                column: "IncomeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_LedgerAccount_ParentId",
                table: "LedgerAccount",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_FiscalPeriod_FiscalYear_FiscalYearId",
                table: "FiscalPeriod",
                column: "FiscalYearId",
                principalTable: "FiscalYear",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FiscalPeriod_FiscalYear_FiscalYearId",
                table: "FiscalPeriod");

            migrationBuilder.DropTable(
                name: "JournalEntryLine");

            migrationBuilder.DropTable(
                name: "LedgerAccount");

            migrationBuilder.DropTable(
                name: "FiscalYear");

            migrationBuilder.DropTable(
                name: "JournalEntry");

            migrationBuilder.DropTable(
                name: "FiscalPeriod");
        }
    }
}
