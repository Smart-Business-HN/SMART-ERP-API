using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMART.ERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bank",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ItIsNationalBank = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bank", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Description = table.Column<string>(type: "varchar(200)", nullable: true),
                    Logo = table.Column<string>(type: "varchar(max)", nullable: true),
                    Background = table.Column<string>(type: "varchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Slug = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Image = table.Column<string>(type: "varchar(max)", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    IsPartCategory = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Country",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Abbreviation = table.Column<string>(type: "varchar(10)", nullable: false),
                    PhoneNumberCode = table.Column<string>(type: "varchar(20)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataSheet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsOutstanding = table.Column<bool>(type: "bit", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSheet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gender",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gender", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Heading",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heading", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterestLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestLevel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InternalDocument",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalDocument", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryInputType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryInputType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoicePaymentType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePaymentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogRecovery",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "varchar(50)", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogRecovery", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LossReason",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LossReason", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MajorExpenseAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MajorExpenseAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MajorIncomeAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MajorIncomeAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaAdCampaign",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", nullable: false),
                    Name = table.Column<string>(type: "varchar(150)", nullable: false),
                    Lifetime_Budget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Stop_Time = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaAdCampaign", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetaConversations",
                columns: table => new
                {
                    Phone = table.Column<string>(type: "varchar(30)", nullable: false),
                    Expiration = table.Column<int>(type: "int", nullable: false),
                    ExpectingInfo = table.Column<bool>(type: "bit", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaConversations", x => x.Phone);
                });

            migrationBuilder.CreateTable(
                name: "OpportunityStep",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityStep", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProspectStep",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProspectStep", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Selector = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocialReason",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialReason", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tax",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,0)", precision: 18, scale: 0, nullable: false),
                    TextForDocuments = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tax", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeActivity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeActivity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeOfPaymentMethod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeOfPaymentMethod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeOrigin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsProspectOrigin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeOrigin", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeProvider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeProvider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TypeStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitOfMeasurement",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Abreviation = table.Column<string>(type: "varchar(10)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasurement", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WinReason",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WinReason", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InternalBankAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CurrentAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalBankAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InternalBankAccount_Bank_BankId",
                        column: x => x.BankId,
                        principalTable: "Bank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Subcategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Slug = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subcategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Region_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExchangeRate_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prefix",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Format = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InternalDocumentId = table.Column<int>(type: "int", nullable: false),
                    ItIsTaken = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prefix", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prefix_InternalDocument_InternalDocumentId",
                        column: x => x.InternalDocumentId,
                        principalTable: "InternalDocument",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpenseAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MajorExpenseAccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseAccount_MajorExpenseAccount_MajorExpenseAccountId",
                        column: x => x.MajorExpenseAccountId,
                        principalTable: "MajorExpenseAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IncomeAccount",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MajorIncomeAccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeAccount_MajorIncomeAccount_MajorIncomeAccountId",
                        column: x => x.MajorIncomeAccountId,
                        principalTable: "MajorIncomeAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Provider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    RTN = table.Column<string>(type: "varchar(16)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", nullable: false),
                    ContactPerson = table.Column<string>(type: "varchar(50)", nullable: true),
                    ContactPhoneNumber = table.Column<string>(type: "varchar(15)", nullable: true),
                    ContactEmail = table.Column<string>(type: "varchar(100)", nullable: true),
                    Address = table.Column<string>(type: "varchar(max)", nullable: false),
                    WebsiteUrl = table.Column<string>(type: "varchar(max)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TypeProviderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Provider_TypeProvider_TypeProviderId",
                        column: x => x.TypeProviderId,
                        principalTable: "TypeProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    TypeStatusId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Status_TypeStatus_TypeStatusId",
                        column: x => x.TypeStatusId,
                        principalTable: "TypeStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    RegionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Department_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Department_Region_RegionId",
                        column: x => x.RegionId,
                        principalTable: "Region",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinancingPlan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Description = table.Column<string>(type: "varchar(600)", nullable: false),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancingPlan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancingPlan_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyPurchaseDeclaration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Period = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    TotalPurchaseBills = table.Column<int>(type: "int", nullable: false),
                    Exempt = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Exonerated = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxedAt15Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxedAt18Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes15Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes18Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalTaxes = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyPurchaseDeclaration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyPurchaseDeclaration_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NonBillableExpense",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpenseAccountId = table.Column<int>(type: "int", nullable: false),
                    Outstanding = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PrefixId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ExpenseCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonBillableExpense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NonBillableExpense_ExpenseAccount_ExpenseAccountId",
                        column: x => x.ExpenseAccountId,
                        principalTable: "ExpenseAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NonBillableExpense_Prefix_PrefixId",
                        column: x => x.PrefixId,
                        principalTable: "Prefix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NonBillableExpense_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NonBillableExpense_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(20)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Description = table.Column<string>(type: "varchar(max)", nullable: true),
                    Brochure = table.Column<string>(type: "varchar(max)", nullable: true),
                    VirtualTour = table.Column<string>(type: "varchar(max)", nullable: true),
                    UrlYoutube = table.Column<string>(type: "varchar(max)", nullable: true),
                    IsFatherProduct = table.Column<bool>(type: "bit", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RecomendedSalePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MinStock = table.Column<int>(type: "int", nullable: false),
                    CurrentStock = table.Column<int>(type: "int", nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: false),
                    UnitOfMeasurementId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ShowInEcommerce = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    EcommerceDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_Brand_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Subcategory_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "Subcategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Tax_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Tax",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Product_UnitOfMeasurement_UnitOfMeasurementId",
                        column: x => x.UnitOfMeasurementId,
                        principalTable: "UnitOfMeasurement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", x => x.Id);
                    table.ForeignKey(
                        name: "FK_City_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EcommerceUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    GenderId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BirthDay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    MasterPasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    MasterPasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    LastPasswordChange = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CustomerTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EcommerceUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EcommerceUser_CustomerType_CustomerTypeId",
                        column: x => x.CustomerTypeId,
                        principalTable: "CustomerType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EcommerceUser_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EcommerceUser_Gender_GenderId",
                        column: x => x.GenderId,
                        principalTable: "Gender",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FullName = table.Column<string>(type: "varchar(50)", nullable: false),
                    Subject = table.Column<string>(type: "varchar(50)", nullable: false),
                    Email = table.Column<string>(type: "varchar(30)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: false),
                    MessageContent = table.Column<string>(type: "varchar(max)", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Message_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NonBillableExpensePayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NonBillableExpenseId = table.Column<int>(type: "int", nullable: false),
                    TypeOfPaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InternalBankAccountId = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Attachment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonBillableExpensePayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NonBillableExpensePayment_InternalBankAccount_InternalBankAccountId",
                        column: x => x.InternalBankAccountId,
                        principalTable: "InternalBankAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NonBillableExpensePayment_NonBillableExpense_NonBillableExpenseId",
                        column: x => x.NonBillableExpenseId,
                        principalTable: "NonBillableExpense",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NonBillableExpensePayment_TypeOfPaymentMethod_TypeOfPaymentMethodId",
                        column: x => x.TypeOfPaymentMethodId,
                        principalTable: "TypeOfPaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HeroSlider",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Position = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeroSlider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroSlider_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HeroSlider_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductDataSheet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(50)", nullable: false),
                    DataSheetId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDataSheet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDataSheet_DataSheet_DataSheetId",
                        column: x => x.DataSheetId,
                        principalTable: "DataSheet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductDataSheet_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductFeature",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(50)", nullable: false),
                    Description = table.Column<string>(type: "varchar(600)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFeature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductFeature_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "varchar(400)", nullable: false),
                    Url = table.Column<string>(type: "varchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImage_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductCode = table.Column<string>(type: "varchar(20)", nullable: false),
                    ProductName = table.Column<string>(type: "varchar(50)", nullable: false),
                    FatherProductId = table.Column<int>(type: "int", nullable: false),
                    FatherProductCode = table.Column<string>(type: "varchar(20)", nullable: false),
                    FatherProductName = table.Column<string>(type: "varchar(50)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductParts_Product_FatherProductId",
                        column: x => x.FatherProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductParts_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdvisorDepartment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvisorDepartment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvisorDepartment_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdvisorGoal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Goal = table.Column<decimal>(type: "decimal(18,0)", precision: 18, scale: 0, nullable: false),
                    InitDate = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2(0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvisorGoal", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banner",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "varchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "varchar(max)", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banner", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BillPayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    TypeOfPaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InternalBankAccountId = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Attachment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillPayment_InternalBankAccount_InternalBankAccountId",
                        column: x => x.InternalBankAccountId,
                        principalTable: "InternalBankAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillPayment_TypeOfPaymentMethod_TypeOfPaymentMethodId",
                        column: x => x.TypeOfPaymentMethodId,
                        principalTable: "TypeOfPaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BranchOffice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Email = table.Column<string>(type: "varchar(30)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: false),
                    Address = table.Column<string>(type: "varchar(150)", nullable: false),
                    Lat = table.Column<float>(type: "real", nullable: false),
                    Lng = table.Column<float>(type: "real", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    MapsId = table.Column<string>(type: "varchar(50)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    IsMainBranchOffice = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchOffice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchOffice_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cai",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BranchOfficeId = table.Column<int>(type: "int", nullable: true),
                    Prefix = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Identificator = table.Column<string>(type: "nvarchar(37)", maxLength: 37, nullable: false),
                    StartCorrelative = table.Column<int>(type: "int", nullable: false),
                    EndCorrelative = table.Column<int>(type: "int", nullable: false),
                    CurrentCorrelative = table.Column<int>(type: "int", nullable: false),
                    AvailableInvoices = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsGeneralCai = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cai", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cai_BranchOffice_BranchOfficeId",
                        column: x => x.BranchOfficeId,
                        principalTable: "BranchOffice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "varchar(30)", nullable: false),
                    FullName = table.Column<string>(type: "varchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "varchar(30)", nullable: false),
                    LastName = table.Column<string>(type: "varchar(30)", nullable: false),
                    Photo = table.Column<string>(type: "varchar(max)", nullable: true),
                    Email = table.Column<string>(type: "varchar(30)", nullable: false),
                    ConfirmedEmail = table.Column<bool>(type: "bit", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(10)", nullable: false),
                    ConfirmedPhoneNumber = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    MasterPasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    MasterPasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    GenderId = table.Column<int>(type: "int", nullable: false),
                    BranchOfficeId = table.Column<int>(type: "int", nullable: true),
                    SalesGoal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_BranchOffice_BranchOfficeId",
                        column: x => x.BranchOfficeId,
                        principalTable: "BranchOffice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_Gender_GenderId",
                        column: x => x.GenderId,
                        principalTable: "Gender",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(50)", nullable: false),
                    Address = table.Column<string>(type: "varchar(max)", nullable: false),
                    AboutUs = table.Column<string>(type: "varchar(300)", nullable: false),
                    FacebookUrl = table.Column<string>(type: "varchar(150)", nullable: true),
                    TwitterUrl = table.Column<string>(type: "varchar(150)", nullable: true),
                    InstagramUrl = table.Column<string>(type: "varchar(150)", nullable: true),
                    YoutubeUrl = table.Column<string>(type: "varchar(150)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "date", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "date", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CaiId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Company_Cai_CaiId",
                        column: x => x.CaiId,
                        principalTable: "Cai",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DailyClose",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    BranchOfficeId = table.Column<int>(type: "int", nullable: false),
                    CaiId = table.Column<int>(type: "int", nullable: false),
                    Exonerated = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Exempt = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxedAt15Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxedAt18Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes15Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes18Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SpotSales = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreditSales = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CashSalesPayments = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreditSalesPayments = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalIncomes = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NumberOfInvoices = table.Column<int>(type: "int", nullable: false),
                    StartCorrelative = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false),
                    EndCorrelative = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyClose", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyClose_BranchOffice_BranchOfficeId",
                        column: x => x.BranchOfficeId,
                        principalTable: "BranchOffice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DailyClose_Cai_CaiId",
                        column: x => x.CaiId,
                        principalTable: "Cai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DNI = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RTN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Company = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConstitutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    ConfirmedEmail = table.Column<bool>(type: "bit", nullable: false),
                    SecondaryEmail = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ConfirmedPhoneNumber = table.Column<bool>(type: "bit", nullable: false),
                    SecondaryPhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CivilStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CustomerTypeId = table.Column<int>(type: "int", nullable: false),
                    GenderId = table.Column<int>(type: "int", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: true),
                    SocialReasonId = table.Column<int>(type: "int", nullable: true),
                    HeadingId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    IsHisOwnContactPerson = table.Column<bool>(type: "bit", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactPersonPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactPersonEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HasChangedPassword = table.Column<bool>(type: "bit", nullable: false),
                    HasEcommercePorfile = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customer_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customer_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customer_CustomerType_CustomerTypeId",
                        column: x => x.CustomerTypeId,
                        principalTable: "CustomerType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customer_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customer_Gender_GenderId",
                        column: x => x.GenderId,
                        principalTable: "Gender",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customer_Heading_HeadingId",
                        column: x => x.HeadingId,
                        principalTable: "Heading",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customer_SocialReason_SocialReasonId",
                        column: x => x.SocialReasonId,
                        principalTable: "SocialReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Customer_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LogSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisterDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    IP = table.Column<string>(type: "varchar(50)", nullable: false),
                    DeviceInfo = table.Column<string>(type: "varchar(150)", nullable: false),
                    Lat = table.Column<string>(type: "varchar(50)", nullable: true),
                    Lng = table.Column<string>(type: "varchar(50)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogSession_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Icon = table.Column<string>(type: "varchar(max)", nullable: true),
                    Image = table.Column<string>(type: "varchar(max)", nullable: true),
                    Title = table.Column<string>(type: "varchar(50)", nullable: true),
                    Description = table.Column<string>(type: "varchar(150)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime", nullable: true),
                    Link = table.Column<string>(type: "varchar(150)", nullable: true),
                    UseRouter = table.Column<bool>(type: "bit", nullable: false),
                    Read = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notification_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OpportunitySchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OpportunityAge = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunitySchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpportunitySchedules_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prospect",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "varchar(150)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", nullable: true),
                    HeadingName = table.Column<string>(type: "varchar(50)", nullable: false),
                    TypeOriginId = table.Column<int>(type: "int", nullable: false),
                    SocialReasonName = table.Column<string>(type: "varchar(50)", nullable: false),
                    ProspectStepId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "varchar(150)", nullable: true),
                    Observation = table.Column<string>(type: "varchar(1000)", nullable: true),
                    MetaAdCampaignId = table.Column<string>(type: "varchar(50)", nullable: true),
                    PostalCode = table.Column<int>(type: "int", nullable: true),
                    WebsiteUrl = table.Column<string>(type: "varchar(max)", nullable: true),
                    GenderId = table.Column<int>(type: "int", nullable: true),
                    PreferredContactMethod = table.Column<string>(type: "varchar(50)", nullable: true),
                    AccountHN = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactPerson = table.Column<string>(type: "varchar(50)", nullable: true),
                    ContactPersonPhone = table.Column<string>(type: "varchar(15)", nullable: true),
                    ContactPersonEmail = table.Column<string>(type: "varchar(30)", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(150)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(150)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prospect", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prospect_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prospect_Country_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Country",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prospect_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prospect_Gender_GenderId",
                        column: x => x.GenderId,
                        principalTable: "Gender",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Prospect_MetaAdCampaign_MetaAdCampaignId",
                        column: x => x.MetaAdCampaignId,
                        principalTable: "MetaAdCampaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prospect_ProspectStep_ProspectStepId",
                        column: x => x.ProspectStepId,
                        principalTable: "ProspectStep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Prospect_TypeOrigin_TypeOriginId",
                        column: x => x.TypeOriginId,
                        principalTable: "TypeOrigin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prospect_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Warehouse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BranchOfficeId = table.Column<int>(type: "int", nullable: true),
                    IsGeneralWarehouse = table.Column<bool>(type: "bit", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Warehouse_BranchOffice_BranchOfficeId",
                        column: x => x.BranchOfficeId,
                        principalTable: "BranchOffice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Warehouse_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Warehouse_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Opinion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(50)", nullable: false),
                    Image = table.Column<string>(type: "varchar(max)", nullable: true),
                    Observation = table.Column<string>(type: "varchar(300)", nullable: false),
                    Customer = table.Column<string>(type: "varchar(50)", nullable: false),
                    Charge = table.Column<string>(type: "varchar(50)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opinion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Opinion_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ResumePayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DailyCloseId = table.Column<int>(type: "int", nullable: false),
                    TypeOfPaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumePayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumePayment_DailyClose_DailyCloseId",
                        column: x => x.DailyCloseId,
                        principalTable: "DailyClose",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResumePayment_TypeOfPaymentMethod_TypeOfPaymentMethodId",
                        column: x => x.TypeOfPaymentMethodId,
                        principalTable: "TypeOfPaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryDirection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AdditionalInformation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsFavorite = table.Column<bool>(type: "bit", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryDirection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryDirection_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryDirection_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Opportunity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(30)", nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ExpectedClosingDate = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    ClosingDate = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    QtyItems = table.Column<int>(type: "int", nullable: false),
                    ProbabilityPercentage = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "varchar(600)", nullable: true),
                    ApplyOnCredit = table.Column<bool>(type: "bit", nullable: false),
                    RecommendedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    OpportunityType = table.Column<string>(type: "varchar(50)", nullable: true),
                    InterestLevelId = table.Column<int>(type: "int", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    OpportunityStepId = table.Column<int>(type: "int", nullable: false),
                    TypeOriginId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2(0)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LossReasonId = table.Column<int>(type: "int", nullable: true),
                    WinReasonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Opportunity_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Opportunity_InterestLevel_InterestLevelId",
                        column: x => x.InterestLevelId,
                        principalTable: "InterestLevel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Opportunity_LossReason_LossReasonId",
                        column: x => x.LossReasonId,
                        principalTable: "LossReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Opportunity_OpportunityStep_OpportunityStepId",
                        column: x => x.OpportunityStepId,
                        principalTable: "OpportunityStep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Opportunity_TypeOrigin_TypeOriginId",
                        column: x => x.TypeOriginId,
                        principalTable: "TypeOrigin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Opportunity_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Opportunity_WinReason_WinReasonId",
                        column: x => x.WinReasonId,
                        principalTable: "WinReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WishList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(30)", nullable: false),
                    CantItems = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishList_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WishList_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProspectQuoteProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProspectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProspectQuoteProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProspectQuoteProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProspectQuoteProduct_Prospect_ProspectId",
                        column: x => x.ProspectId,
                        principalTable: "Prospect",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryDistribution",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryDistribution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryDistribution_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryDistribution_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OpportunityActivity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "varchar(300)", nullable: false),
                    InitDate = table.Column<DateTime>(type: "datetime2(2)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2(2)", nullable: false),
                    TypeActivityId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OpportunityId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2(2)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2(2)", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    GCEventId = table.Column<string>(type: "varchar(300)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpportunityActivity_Opportunity_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpportunityActivity_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpportunityActivity_TypeActivity_TypeActivityId",
                        column: x => x.TypeActivityId,
                        principalTable: "TypeActivity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpportunityActivity_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OpportunityComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "varchar(600)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OpportunityId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpportunityComment_Opportunity_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpportunityComment_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OpportunityDocument",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    OpportunityId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpportunityDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpportunityDocument_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpportunityDocument_Opportunity_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OpportunityDocument_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuoteProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpportunityId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SalePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuoteProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuoteProduct_Opportunity_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuoteProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "varchar(50)", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CantItems = table.Column<int>(type: "int", nullable: true),
                    TotalToPay = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OpportunityId = table.Column<int>(type: "int", nullable: false),
                    FinancingPlanId = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(50)", nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificatedBy = table.Column<string>(type: "varchar(50)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrder_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrder_FinancingPlan_FinancingPlanId",
                        column: x => x.FinancingPlanId,
                        principalTable: "FinancingPlan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrder_Opportunity_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrder_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WishListProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WishListId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishListProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishListProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WishListProduct_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WishListProduct_WishList_WishListId",
                        column: x => x.WishListId,
                        principalTable: "WishList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SaleOrderProduct",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SaleOrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleOrderProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleOrderProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleOrderProduct_SaleOrder_SaleOrderId",
                        column: x => x.SaleOrderId,
                        principalTable: "SaleOrder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cart",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EcommerceUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DestinationQuotationId = table.Column<int>(type: "int", nullable: true),
                    ConvertionDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cart", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cart_EcommerceUser_EcommerceUserId",
                        column: x => x.EcommerceUserId,
                        principalTable: "EcommerceUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, computedColumnSql: "[Quantity] * [UnitPrice] - ISNULL([Discount], 0)"),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItem_Cart_CartId",
                        column: x => x.CartId,
                        principalTable: "Cart",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeclaratedPurchaseBill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonthlyPurchaseDeclarationId = table.Column<int>(type: "int", nullable: false),
                    PurchaseBillId = table.Column<int>(type: "int", nullable: false),
                    ProviderRTN = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    BillDate = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Cai = table.Column<string>(type: "nvarchar(37)", maxLength: 37, nullable: false),
                    Establishment = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    EmissionPoint = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    KindOfDocument = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Correlative = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    PurchaseWithOce = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    ResolutionNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ResolutionDate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Exempt = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Exonerated = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxedAt15Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxedAt18Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes15Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes18Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclaratedPurchaseBill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeclaratedPurchaseBill_MonthlyPurchaseDeclaration_MonthlyPurchaseDeclarationId",
                        column: x => x.MonthlyPurchaseDeclarationId,
                        principalTable: "MonthlyPurchaseDeclaration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryInput",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryInputTypeId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    PrefixId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    PurchaseOrderOriginId = table.Column<int>(type: "int", nullable: true),
                    ProductReturnId = table.Column<int>(type: "int", nullable: true),
                    SurplusInventoryId = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryInput", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryInput_InventoryInputType_InventoryInputTypeId",
                        column: x => x.InventoryInputTypeId,
                        principalTable: "InventoryInputType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryInput_Prefix_PrefixId",
                        column: x => x.PrefixId,
                        principalTable: "Prefix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryInput_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryInput_Warehouse_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InventoryInputId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    UnitProductPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductEntry_InventoryInput_InventoryInputId",
                        column: x => x.InventoryInputId,
                        principalTable: "InventoryInput",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductEntry_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaiId = table.Column<int>(type: "int", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false),
                    QuotationOriginId = table.Column<int>(type: "int", nullable: true),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchOfficeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchaseOrderCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SagCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExemptOrderNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExemptedRegistrationCertificateNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Exempt = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Exonerated = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxedAt15Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxedAt18Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes15Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes18Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Outstanding = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    InvoicePaymentTypeId = table.Column<int>(type: "int", nullable: false),
                    ExpectedPaymentDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoice_BranchOffice_BranchOfficeId",
                        column: x => x.BranchOfficeId,
                        principalTable: "BranchOffice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoice_Cai_CaiId",
                        column: x => x.CaiId,
                        principalTable: "Cai",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoice_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoice_InvoicePaymentType_InvoicePaymentTypeId",
                        column: x => x.InvoicePaymentTypeId,
                        principalTable: "InvoicePaymentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoice_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Invoice_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductSold",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProductDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    Taxes = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalLine = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSold", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSold_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductSold_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductSold_Tax_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Tax",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quotation",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuotationCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BranchOfficeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    PrefixId = table.Column<int>(type: "int", nullable: false),
                    Profitability = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    InvoiceDestinationId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InsertedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModificatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotation_BranchOffice_BranchOfficeId",
                        column: x => x.BranchOfficeId,
                        principalTable: "BranchOffice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotation_Customer_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotation_Invoice_InvoiceDestinationId",
                        column: x => x.InvoiceDestinationId,
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotation_Prefix_PrefixId",
                        column: x => x.PrefixId,
                        principalTable: "Prefix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotation_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotation_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductOffered",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuotationId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProductDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    Taxes = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalLine = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOffered", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductOffered_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductOffered_Quotation_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductOffered_Tax_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Tax",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPurchasePriceLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchaseBillOriginId = table.Column<int>(type: "int", nullable: false),
                    UnitsPurchased = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPurchasePriceLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPurchasePriceLog_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductToPurchase",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseOrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    Taxes = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalLine = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductToPurchase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductToPurchase_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductToPurchase_Tax_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Tax",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseBill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseBillCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(19)", maxLength: 19, nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cai = table.Column<string>(type: "nvarchar(37)", maxLength: 37, nullable: false),
                    PurchaseOrderOriginId = table.Column<int>(type: "int", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Exempt = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Exonerated = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxedAt15Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TaxedAt18Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes15Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Taxes18Percent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Outstanding = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InventoryInputDestinationId = table.Column<int>(type: "int", nullable: true),
                    PrefixId = table.Column<int>(type: "int", nullable: false),
                    ExpenseAccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseBill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseBill_ExpenseAccount_ExpenseAccountId",
                        column: x => x.ExpenseAccountId,
                        principalTable: "ExpenseAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseBill_Prefix_PrefixId",
                        column: x => x.PrefixId,
                        principalTable: "Prefix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseBill_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseBill_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseBillPayment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseBillId = table.Column<int>(type: "int", nullable: false),
                    TypeOfPaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    InternalBankAccountId = table.Column<int>(type: "int", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Attachment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseBillPayment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseBillPayment_InternalBankAccount_InternalBankAccountId",
                        column: x => x.InternalBankAccountId,
                        principalTable: "InternalBankAccount",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseBillPayment_PurchaseBill_PurchaseBillId",
                        column: x => x.PurchaseBillId,
                        principalTable: "PurchaseBill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseBillPayment_TypeOfPaymentMethod_TypeOfPaymentMethodId",
                        column: x => x.TypeOfPaymentMethodId,
                        principalTable: "TypeOfPaymentMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProviderId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BranchOfficeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observations = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    PrefixId = table.Column<int>(type: "int", nullable: false),
                    PurchaseBillDestinationId = table.Column<int>(type: "int", nullable: true),
                    InventoryInputDestinationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_BranchOffice_BranchOfficeId",
                        column: x => x.BranchOfficeId,
                        principalTable: "BranchOffice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_InventoryInput_InventoryInputDestinationId",
                        column: x => x.InventoryInputDestinationId,
                        principalTable: "InventoryInput",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_Prefix_PrefixId",
                        column: x => x.PrefixId,
                        principalTable: "Prefix",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_Provider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Provider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_PurchaseBill_PurchaseBillDestinationId",
                        column: x => x.PurchaseBillDestinationId,
                        principalTable: "PurchaseBill",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrder_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvisorDepartment_DepartmentId",
                table: "AdvisorDepartment",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvisorDepartment_UserId",
                table: "AdvisorDepartment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvisorGoal_UserId",
                table: "AdvisorGoal",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Banner_CompanyId",
                table: "Banner",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BillPayment_InternalBankAccountId",
                table: "BillPayment",
                column: "InternalBankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BillPayment_InvoiceId",
                table: "BillPayment",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_BillPayment_TypeOfPaymentMethodId",
                table: "BillPayment",
                column: "TypeOfPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchOffice_CityId",
                table: "BranchOffice",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchOffice_CompanyId",
                table: "BranchOffice",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Cai_BranchOfficeId",
                table: "Cai",
                column: "BranchOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_DestinationQuotationId",
                table: "Cart",
                column: "DestinationQuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_Cart_EcommerceUserId",
                table: "Cart",
                column: "EcommerceUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_CartId",
                table: "CartItem",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_ProductId",
                table: "CartItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_City_DepartmentId",
                table: "City",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_CaiId",
                table: "Company",
                column: "CaiId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CountryId",
                table: "Customer",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CurrencyId",
                table: "Customer",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_CustomerTypeId",
                table: "Customer",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_DepartmentId",
                table: "Customer",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_GenderId",
                table: "Customer",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_HeadingId",
                table: "Customer",
                column: "HeadingId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_SocialReasonId",
                table: "Customer",
                column: "SocialReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Customer_UserId",
                table: "Customer",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyClose_BranchOfficeId",
                table: "DailyClose",
                column: "BranchOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyClose_CaiId",
                table: "DailyClose",
                column: "CaiId");

            migrationBuilder.CreateIndex(
                name: "IX_DeclaratedPurchaseBill_MonthlyPurchaseDeclarationId",
                table: "DeclaratedPurchaseBill",
                column: "MonthlyPurchaseDeclarationId");

            migrationBuilder.CreateIndex(
                name: "IX_DeclaratedPurchaseBill_PurchaseBillId",
                table: "DeclaratedPurchaseBill",
                column: "PurchaseBillId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDirection_CityId",
                table: "DeliveryDirection",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDirection_CustomerId",
                table: "DeliveryDirection",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_CountryId",
                table: "Department",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Department_RegionId",
                table: "Department",
                column: "RegionId");

            migrationBuilder.CreateIndex(
                name: "IX_EcommerceUser_CustomerTypeId",
                table: "EcommerceUser",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EcommerceUser_DepartmentId",
                table: "EcommerceUser",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EcommerceUser_GenderId",
                table: "EcommerceUser",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRate_CurrencyId",
                table: "ExchangeRate",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseAccount_MajorExpenseAccountId",
                table: "ExpenseAccount",
                column: "MajorExpenseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancingPlan_ProviderId",
                table: "FinancingPlan",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroSlider_CategoryId",
                table: "HeroSlider",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroSlider_ProductId",
                table: "HeroSlider",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeAccount_MajorIncomeAccountId",
                table: "IncomeAccount",
                column: "MajorIncomeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalBankAccount_BankId",
                table: "InternalBankAccount",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDistribution_ProductId",
                table: "InventoryDistribution",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDistribution_WarehouseId",
                table: "InventoryDistribution",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryInput_InventoryInputTypeId",
                table: "InventoryInput",
                column: "InventoryInputTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryInput_PrefixId",
                table: "InventoryInput",
                column: "PrefixId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryInput_PurchaseOrderOriginId",
                table: "InventoryInput",
                column: "PurchaseOrderOriginId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryInput_StatusId",
                table: "InventoryInput",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryInput_WarehouseId",
                table: "InventoryInput",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_BranchOfficeId",
                table: "Invoice",
                column: "BranchOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CaiId",
                table: "Invoice",
                column: "CaiId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_CustomerId",
                table: "Invoice",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_InvoicePaymentTypeId",
                table: "Invoice",
                column: "InvoicePaymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_QuotationOriginId",
                table: "Invoice",
                column: "QuotationOriginId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_StatusId",
                table: "Invoice",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_UserId",
                table: "Invoice",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LogSession_UserId",
                table: "LogSession",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_CountryId",
                table: "Message",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_DepartmentId",
                table: "Message",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyPurchaseDeclaration_StatusId",
                table: "MonthlyPurchaseDeclaration",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_NonBillableExpense_ExpenseAccountId",
                table: "NonBillableExpense",
                column: "ExpenseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_NonBillableExpense_PrefixId",
                table: "NonBillableExpense",
                column: "PrefixId");

            migrationBuilder.CreateIndex(
                name: "IX_NonBillableExpense_ProviderId",
                table: "NonBillableExpense",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_NonBillableExpense_StatusId",
                table: "NonBillableExpense",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_NonBillableExpensePayment_InternalBankAccountId",
                table: "NonBillableExpensePayment",
                column: "InternalBankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_NonBillableExpensePayment_NonBillableExpenseId",
                table: "NonBillableExpensePayment",
                column: "NonBillableExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_NonBillableExpensePayment_TypeOfPaymentMethodId",
                table: "NonBillableExpensePayment",
                column: "TypeOfPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId",
                table: "Notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Opinion_CompanyId",
                table: "Opinion",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_CustomerId",
                table: "Opportunity",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_InterestLevelId",
                table: "Opportunity",
                column: "InterestLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_LossReasonId",
                table: "Opportunity",
                column: "LossReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_OpportunityStepId",
                table: "Opportunity",
                column: "OpportunityStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_TypeOriginId",
                table: "Opportunity",
                column: "TypeOriginId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_UserId",
                table: "Opportunity",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunity_WinReasonId",
                table: "Opportunity",
                column: "WinReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityActivity_OpportunityId",
                table: "OpportunityActivity",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityActivity_StatusId",
                table: "OpportunityActivity",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityActivity_TypeActivityId",
                table: "OpportunityActivity",
                column: "TypeActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityActivity_UserId",
                table: "OpportunityActivity",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityComment_OpportunityId",
                table: "OpportunityComment",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityComment_UserId",
                table: "OpportunityComment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityDocument_DocumentTypeId",
                table: "OpportunityDocument",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityDocument_OpportunityId",
                table: "OpportunityDocument",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunityDocument_UserId",
                table: "OpportunityDocument",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OpportunitySchedules_UserId",
                table: "OpportunitySchedules",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prefix_InternalDocumentId",
                table: "Prefix",
                column: "InternalDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_BrandId",
                table: "Product",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProviderId",
                table: "Product",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_StatusId",
                table: "Product",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_SubCategoryId",
                table: "Product",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_TaxId",
                table: "Product",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_UnitOfMeasurementId",
                table: "Product",
                column: "UnitOfMeasurementId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDataSheet_DataSheetId",
                table: "ProductDataSheet",
                column: "DataSheetId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDataSheet_ProductId",
                table: "ProductDataSheet",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductEntry_InventoryInputId",
                table: "ProductEntry",
                column: "InventoryInputId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductEntry_ProductId",
                table: "ProductEntry",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeature_ProductId",
                table: "ProductFeature",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImage_ProductId",
                table: "ProductImage",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOffered_ProductId",
                table: "ProductOffered",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOffered_QuotationId",
                table: "ProductOffered",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductOffered_TaxId",
                table: "ProductOffered",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductParts_FatherProductId",
                table: "ProductParts",
                column: "FatherProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductParts_ProductId",
                table: "ProductParts",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPurchasePriceLog_ProductId",
                table: "ProductPurchasePriceLog",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPurchasePriceLog_PurchaseBillOriginId",
                table: "ProductPurchasePriceLog",
                column: "PurchaseBillOriginId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSold_InvoiceId",
                table: "ProductSold",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSold_ProductId",
                table: "ProductSold",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSold_TaxId",
                table: "ProductSold",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductToPurchase_ProductId",
                table: "ProductToPurchase",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductToPurchase_PurchaseOrderId",
                table: "ProductToPurchase",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductToPurchase_TaxId",
                table: "ProductToPurchase",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_Prospect_CityId",
                table: "Prospect",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Prospect_CountryId",
                table: "Prospect",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Prospect_DepartmentId",
                table: "Prospect",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Prospect_GenderId",
                table: "Prospect",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Prospect_MetaAdCampaignId",
                table: "Prospect",
                column: "MetaAdCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_Prospect_ProspectStepId",
                table: "Prospect",
                column: "ProspectStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Prospect_TypeOriginId",
                table: "Prospect",
                column: "TypeOriginId");

            migrationBuilder.CreateIndex(
                name: "IX_Prospect_UserId",
                table: "Prospect",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProspectQuoteProduct_ProductId",
                table: "ProspectQuoteProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProspectQuoteProduct_ProspectId",
                table: "ProspectQuoteProduct",
                column: "ProspectId");

            migrationBuilder.CreateIndex(
                name: "IX_Provider_TypeProviderId",
                table: "Provider",
                column: "TypeProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBill_ExpenseAccountId",
                table: "PurchaseBill",
                column: "ExpenseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBill_PrefixId",
                table: "PurchaseBill",
                column: "PrefixId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBill_ProviderId",
                table: "PurchaseBill",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBill_PurchaseOrderOriginId",
                table: "PurchaseBill",
                column: "PurchaseOrderOriginId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBill_StatusId",
                table: "PurchaseBill",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBillPayment_InternalBankAccountId",
                table: "PurchaseBillPayment",
                column: "InternalBankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBillPayment_PurchaseBillId",
                table: "PurchaseBillPayment",
                column: "PurchaseBillId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseBillPayment_TypeOfPaymentMethodId",
                table: "PurchaseBillPayment",
                column: "TypeOfPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_BranchOfficeId",
                table: "PurchaseOrder",
                column: "BranchOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_InventoryInputDestinationId",
                table: "PurchaseOrder",
                column: "InventoryInputDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_PrefixId",
                table: "PurchaseOrder",
                column: "PrefixId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_ProviderId",
                table: "PurchaseOrder",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_PurchaseBillDestinationId",
                table: "PurchaseOrder",
                column: "PurchaseBillDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_StatusId",
                table: "PurchaseOrder",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_UserId",
                table: "PurchaseOrder",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_BranchOfficeId",
                table: "Quotation",
                column: "BranchOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_CustomerId",
                table: "Quotation",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_InvoiceDestinationId",
                table: "Quotation",
                column: "InvoiceDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_PrefixId",
                table: "Quotation",
                column: "PrefixId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_StatusId",
                table: "Quotation",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_UserId",
                table: "Quotation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteProduct_OpportunityId",
                table: "QuoteProduct",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_QuoteProduct_ProductId",
                table: "QuoteProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Region_CountryId",
                table: "Region",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumePayment_DailyCloseId",
                table: "ResumePayment",
                column: "DailyCloseId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumePayment_TypeOfPaymentMethodId",
                table: "ResumePayment",
                column: "TypeOfPaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrder_CustomerId",
                table: "SaleOrder",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrder_FinancingPlanId",
                table: "SaleOrder",
                column: "FinancingPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrder_OpportunityId",
                table: "SaleOrder",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrder_StatusId",
                table: "SaleOrder",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderProduct_ProductId",
                table: "SaleOrderProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleOrderProduct_SaleOrderId",
                table: "SaleOrderProduct",
                column: "SaleOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Status_TypeStatusId",
                table: "Status",
                column: "TypeStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Subcategory_CategoryId",
                table: "Subcategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_User_BranchOfficeId",
                table: "User",
                column: "BranchOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_User_GenderId",
                table: "User",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_BranchOfficeId",
                table: "Warehouse",
                column: "BranchOfficeId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_CityId",
                table: "Warehouse",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_UserId",
                table: "Warehouse",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WishList_CustomerId",
                table: "WishList",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_WishList_StatusId",
                table: "WishList",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_WishListProduct_ProductId",
                table: "WishListProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_WishListProduct_StatusId",
                table: "WishListProduct",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_WishListProduct_WishListId",
                table: "WishListProduct",
                column: "WishListId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvisorDepartment_User_UserId",
                table: "AdvisorDepartment",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdvisorGoal_User_UserId",
                table: "AdvisorGoal",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Banner_Company_CompanyId",
                table: "Banner",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BillPayment_Invoice_InvoiceId",
                table: "BillPayment",
                column: "InvoiceId",
                principalTable: "Invoice",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BranchOffice_Company_CompanyId",
                table: "BranchOffice",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Quotation_DestinationQuotationId",
                table: "Cart",
                column: "DestinationQuotationId",
                principalTable: "Quotation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DeclaratedPurchaseBill_PurchaseBill_PurchaseBillId",
                table: "DeclaratedPurchaseBill",
                column: "PurchaseBillId",
                principalTable: "PurchaseBill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryInput_PurchaseOrder_PurchaseOrderOriginId",
                table: "InventoryInput",
                column: "PurchaseOrderOriginId",
                principalTable: "PurchaseOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoice_Quotation_QuotationOriginId",
                table: "Invoice",
                column: "QuotationOriginId",
                principalTable: "Quotation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPurchasePriceLog_PurchaseBill_PurchaseBillOriginId",
                table: "ProductPurchasePriceLog",
                column: "PurchaseBillOriginId",
                principalTable: "PurchaseBill",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductToPurchase_PurchaseOrder_PurchaseOrderId",
                table: "ProductToPurchase",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseBill_PurchaseOrder_PurchaseOrderOriginId",
                table: "PurchaseBill",
                column: "PurchaseOrderOriginId",
                principalTable: "PurchaseOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_City_Department_DepartmentId",
                table: "City");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_Department_DepartmentId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Customer_User_UserId",
                table: "Customer");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoice_User_UserId",
                table: "Invoice");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrder_User_UserId",
                table: "PurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotation_User_UserId",
                table: "Quotation");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_User_UserId",
                table: "Warehouse");

            migrationBuilder.DropForeignKey(
                name: "FK_BranchOffice_Company_CompanyId",
                table: "BranchOffice");

            migrationBuilder.DropForeignKey(
                name: "FK_Quotation_Invoice_InvoiceDestinationId",
                table: "Quotation");

            migrationBuilder.DropForeignKey(
                name: "FK_BranchOffice_City_CityId",
                table: "BranchOffice");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_City_CityId",
                table: "Warehouse");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrder_BranchOffice_BranchOfficeId",
                table: "PurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_Warehouse_BranchOffice_BranchOfficeId",
                table: "Warehouse");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrder_PurchaseBill_PurchaseBillDestinationId",
                table: "PurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrder_Provider_ProviderId",
                table: "PurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryInput_Warehouse_WarehouseId",
                table: "InventoryInput");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryInput_InventoryInputType_InventoryInputTypeId",
                table: "InventoryInput");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryInput_Prefix_PrefixId",
                table: "InventoryInput");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrder_Prefix_PrefixId",
                table: "PurchaseOrder");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryInput_PurchaseOrder_PurchaseOrderOriginId",
                table: "InventoryInput");

            migrationBuilder.DropTable(
                name: "AdvisorDepartment");

            migrationBuilder.DropTable(
                name: "AdvisorGoal");

            migrationBuilder.DropTable(
                name: "Banner");

            migrationBuilder.DropTable(
                name: "BillPayment");

            migrationBuilder.DropTable(
                name: "CartItem");

            migrationBuilder.DropTable(
                name: "DeclaratedPurchaseBill");

            migrationBuilder.DropTable(
                name: "DeliveryDirection");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "ExchangeRate");

            migrationBuilder.DropTable(
                name: "HeroSlider");

            migrationBuilder.DropTable(
                name: "IncomeAccount");

            migrationBuilder.DropTable(
                name: "InventoryDistribution");

            migrationBuilder.DropTable(
                name: "LogRecovery");

            migrationBuilder.DropTable(
                name: "LogSession");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "MetaConversations");

            migrationBuilder.DropTable(
                name: "NonBillableExpensePayment");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "Opinion");

            migrationBuilder.DropTable(
                name: "OpportunityActivity");

            migrationBuilder.DropTable(
                name: "OpportunityComment");

            migrationBuilder.DropTable(
                name: "OpportunityDocument");

            migrationBuilder.DropTable(
                name: "OpportunitySchedules");

            migrationBuilder.DropTable(
                name: "ProductDataSheet");

            migrationBuilder.DropTable(
                name: "ProductEntry");

            migrationBuilder.DropTable(
                name: "ProductFeature");

            migrationBuilder.DropTable(
                name: "ProductImage");

            migrationBuilder.DropTable(
                name: "ProductOffered");

            migrationBuilder.DropTable(
                name: "ProductParts");

            migrationBuilder.DropTable(
                name: "ProductPurchasePriceLog");

            migrationBuilder.DropTable(
                name: "ProductSold");

            migrationBuilder.DropTable(
                name: "ProductToPurchase");

            migrationBuilder.DropTable(
                name: "ProspectQuoteProduct");

            migrationBuilder.DropTable(
                name: "PurchaseBillPayment");

            migrationBuilder.DropTable(
                name: "QuoteProduct");

            migrationBuilder.DropTable(
                name: "ResumePayment");

            migrationBuilder.DropTable(
                name: "SaleOrderProduct");

            migrationBuilder.DropTable(
                name: "WishListProduct");

            migrationBuilder.DropTable(
                name: "Cart");

            migrationBuilder.DropTable(
                name: "MonthlyPurchaseDeclaration");

            migrationBuilder.DropTable(
                name: "MajorIncomeAccount");

            migrationBuilder.DropTable(
                name: "NonBillableExpense");

            migrationBuilder.DropTable(
                name: "TypeActivity");

            migrationBuilder.DropTable(
                name: "DocumentType");

            migrationBuilder.DropTable(
                name: "DataSheet");

            migrationBuilder.DropTable(
                name: "Prospect");

            migrationBuilder.DropTable(
                name: "InternalBankAccount");

            migrationBuilder.DropTable(
                name: "DailyClose");

            migrationBuilder.DropTable(
                name: "TypeOfPaymentMethod");

            migrationBuilder.DropTable(
                name: "SaleOrder");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "WishList");

            migrationBuilder.DropTable(
                name: "EcommerceUser");

            migrationBuilder.DropTable(
                name: "MetaAdCampaign");

            migrationBuilder.DropTable(
                name: "ProspectStep");

            migrationBuilder.DropTable(
                name: "Bank");

            migrationBuilder.DropTable(
                name: "FinancingPlan");

            migrationBuilder.DropTable(
                name: "Opportunity");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropTable(
                name: "Subcategory");

            migrationBuilder.DropTable(
                name: "Tax");

            migrationBuilder.DropTable(
                name: "UnitOfMeasurement");

            migrationBuilder.DropTable(
                name: "InterestLevel");

            migrationBuilder.DropTable(
                name: "LossReason");

            migrationBuilder.DropTable(
                name: "OpportunityStep");

            migrationBuilder.DropTable(
                name: "TypeOrigin");

            migrationBuilder.DropTable(
                name: "WinReason");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "Region");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "Cai");

            migrationBuilder.DropTable(
                name: "InvoicePaymentType");

            migrationBuilder.DropTable(
                name: "Quotation");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Country");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "CustomerType");

            migrationBuilder.DropTable(
                name: "Gender");

            migrationBuilder.DropTable(
                name: "Heading");

            migrationBuilder.DropTable(
                name: "SocialReason");

            migrationBuilder.DropTable(
                name: "City");

            migrationBuilder.DropTable(
                name: "BranchOffice");

            migrationBuilder.DropTable(
                name: "PurchaseBill");

            migrationBuilder.DropTable(
                name: "ExpenseAccount");

            migrationBuilder.DropTable(
                name: "MajorExpenseAccount");

            migrationBuilder.DropTable(
                name: "Provider");

            migrationBuilder.DropTable(
                name: "TypeProvider");

            migrationBuilder.DropTable(
                name: "Warehouse");

            migrationBuilder.DropTable(
                name: "InventoryInputType");

            migrationBuilder.DropTable(
                name: "Prefix");

            migrationBuilder.DropTable(
                name: "InternalDocument");

            migrationBuilder.DropTable(
                name: "PurchaseOrder");

            migrationBuilder.DropTable(
                name: "InventoryInput");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "TypeStatus");
        }
    }
}
