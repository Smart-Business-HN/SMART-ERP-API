using Microsoft.EntityFrameworkCore;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Infrastructure
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<AdvisorDepartment> AdvisorDepartments { get; set; } = null!;
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<Banner> Banners { get; set; } = null!;
        public DbSet<BranchOffices> BranchOffices { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<Currency> Currencies { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<DataSheet> DataSheets { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<ExchangeRate> ExchangeRates { get; set; } = null!;
        public DbSet<FinancingPlan> FinancingPlans { get; set; } = null!;
        public DbSet<Gender> Genders { get; set; } = null!;
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Region> Regions { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<MetaConversations> MetaConversations { get; set; } = null!;
        public DbSet<MetaAdCampaign> MetaAdCampaigns { get; set; } = null!;
        public DbSet<Opinion> Opinions { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<ProductDataSheet> ProductDataSheets { get; set; } = null!;
        public DbSet<ProductFeature> ProductFeatures { get; set; } = null!;
        public DbSet<ProductImage> ProductImages { get; set; } = null!;
        public DbSet<ProductPart> ProductParts { get; set; } = null!;
        public DbSet<ProductSubcategory> ProductSubcategories { get; set; } = null!;
        public DbSet<Subcategory> Subcategories { get; set; } = null!;
        public DbSet<Provider> Providers { get; set; } = null!;
        public DbSet<Prospect> Prospects { get; set; } = null!;
        public DbSet<ProspectStep> ProspectSteps { get; set; } = null!;
        public DbSet<ProspectQuoteProduct> ProspectQuoteProducts { get; set; } = null!;
        public DbSet<QuoteProduct> QuoteProducts { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<SaleOrder> SaleOrders { get; set; } = null!;
        public DbSet<Opportunity> Opportunities { get; set; } = null!;
        public DbSet<Status> Statuses { get; set; } = null!;
        public DbSet<TypeStatus> TypeStatuses { get; set; } = null!;
        public DbSet<UnitOfMeasurement> UnitOfMeasurements { get; set; } = null!;
        public DbSet<WishList> WishLists { get; set; } = null!;
        public DbSet<WishListProduct> WishListProducts { get; set; } = null!;
        public DbSet<LogSession> LogSessions { get; set; } = null!;
        public DbSet<LogRecovery> LogRecoveries { get; set; } = null!;
        public DbSet<HeroSlider> HeroSliders { get; set; } = null!;
        public DbSet<Notification> Notificactions { get; set; } = null!;
        public DbSet<SaleOrderProduct> SaleOrderProducts { get; set; } = null!;
        public DbSet<TypeOrigin> TypeOrigins { get; set; } = null!;
        public DbSet<DocumentType> DocumentTypes { get; set; } = null!;
        public DbSet<InterestLevel> InterestLevels { get; set; } = null!;
        public DbSet<OpportunityActivity> OpportunityActivities { get; set; } = null!;
        public DbSet<OpportunityComment> OpportunityComments { get; set; } = null!;
        public DbSet<OpportunityDocument> OpportunityDocuments { get; set; } = null!;
        public DbSet<OpportunitySchedules> OpportunitySchedules { get; set; } = null!;
        public DbSet<TypeActivity> TypeActivities { get; set; } = null!;
        public DbSet<LossReason> LossReasons { get; set; } = null!;
        public DbSet<WinReason> WinReasons { get; set; } = null!;
        public DbSet<AdvisorGoal> AdvisorGoals { get; set; } = null!;
        public DbSet<Cai> Cais { get; set; } = null!;
        public DbSet<InternalDocument> InternalDocuments { get; set; } = null!;
        public DbSet<Prefix> Prefixes { get; set; } = null!;
        public DbSet<CustomerType> CustomerTypes { get; set; } = null!;
        public DbSet<PriceList> PriceLists { get; set; } = null!;
        public DbSet<PriceListItem> PriceListItems { get; set; } = null!;
        public DbSet<Quotation> Quotations { get; set; } = null!;
        public DbSet<ProductOffered> ProductOffereds { get; set; } = null!;
        public DbSet<Warehouse> Warehouses { get; set; } = null!;
        public DbSet<InventoryDistribution> InventoryDistributions { get; set; } = null!;
        public DbSet<WarehouseType> WarehouseTypes { get; set; } = null!;
        public DbSet<ProviderWarehouse> ProviderWarehouses { get; set; } = null!;
        public DbSet<ShippingCostConfiguration> ShippingCostConfigurations { get; set; } = null!;
        public DbSet<VirtualStockImport> VirtualStockImports { get; set; } = null!;
        public DbSet<VirtualStockImportDetail> VirtualStockImportDetails { get; set; } = null!;
        public DbSet<InventoryInput> InventoryInputs { get; set; } = null!;
        public DbSet<InventoryInputType> InventoryInputTypes { get; set; } = null!;
        public DbSet<ProductEntry> ProductEntries { get; set; } = null!;
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<ProductSold> ProductsSold { get; set; } = null!;
        public DbSet<Bank> Banks { get; set; } = null!;
        public DbSet<InternalBankAccount> InternalBankAccounts { get; set; } = null!;
        public DbSet<BillPayment> BillPayments { get; set; } = null!;
        public DbSet<TypeProvider> TypeProviders { get; set; } = null!;
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;
        public DbSet<PurchaseBill> PurchaseBills { get; set; } = null!;
        public DbSet<ProductPurchasePriceLog> ProductPurchasePriceLog { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<NonBillableExpense> NonBillableExpenses { get; set; } = null!;
        public DbSet<NonBillableExpensePayment> NonBillableExpensePayments { get; set; } = null!;
        public DbSet<CreditCardPayment> CreditCardPayments { get; set; } = null!;
        public DbSet<DailyClose> DailyCloses { get; set; } = null!;
        public DbSet<ResumePayment> ResumePayments { get; set; } = null!;
        public DbSet<MonthlyPurchaseDeclaration> MonthlyPurchaseDeclarations { get; set; } = null!;
        public DbSet<DeclaratedPurchaseBill> DeclaratedPurchaseBills { get; set; } = null!;
        public DbSet<MonthlySaleDeclaration> MonthlySaleDeclarations { get; set; } = null!;
        public DbSet<DeclaredSaleInvoice> DeclaredSaleInvoices { get; set; } = null!;
        public DbSet<InvoicePaymentType> InvoicePaymentTypes { get; set; } = null!;
        public DbSet<Discount> Discounts { get; set; } = null!;
        public DbSet<EcommerceUser> EcommerceUsers { get; set; } = null!;
        public DbSet<LogEcommerceUser> LogEcommerceUsers { get; set; } = null!;
        public DbSet<AssociatedCompany> AssociatedCompanies { get; set; } = null!;
        public DbSet<PaymentMethod> PaymentMethods { get; set; } = null!;
        public DbSet<ProjectAttachmentCategory> ProjectAttachmentCategories { get; set; } = null!;
        public DbSet<ProjectAttachment> ProjectAttachments { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<ChatSession> ChatSessions { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public DbSet<RecurringInvoiceTemplate> RecurringInvoiceTemplates { get; set; } = null!;
        public DbSet<RecurringInvoiceTemplateItem> RecurringInvoiceTemplateItems { get; set; } = null!;
        public DbSet<RecurringInvoiceLog> RecurringInvoiceLogs { get; set; } = null!;
        public DbSet<QuotationSnapshot> QuotationSnapshots { get; set; } = null!;
        public DbSet<QuotationComment> QuotationComments { get; set; } = null!;
        public DbSet<QuotationItemObservation> QuotationItemObservations { get; set; } = null!;
        public DbSet<InventoryEntry> InventoryEntries { get; set; } = null!;
        public DbSet<InventoryEntryItem> InventoryEntryItems { get; set; } = null!;
        public DbSet<InventoryMovement> InventoryMovements { get; set; } = null!;
        public DbSet<InventoryExit> InventoryExits { get; set; } = null!;
        public DbSet<InventoryExitItem> InventoryExitItems { get; set; } = null!;
        public DbSet<WarehouseTransfer> WarehouseTransfers { get; set; } = null!;
        public DbSet<WarehouseTransferItem> WarehouseTransferItems { get; set; } = null!;
        public DbSet<LedgerAccount> LedgerAccounts { get; set; } = null!;
        public DbSet<FiscalYear> FiscalYears { get; set; } = null!;
        public DbSet<FiscalPeriod> FiscalPeriods { get; set; } = null!;
        public DbSet<JournalEntry> JournalEntries { get; set; } = null!;
        public DbSet<JournalEntryLine> JournalEntryLines { get; set; } = null!;
        public DbSet<AccountingSettings> AccountingSettings { get; set; } = null!;
        public DbSet<AccountingMapping> AccountingMappings { get; set; } = null!;
        public DbSet<CostCenter> CostCenters { get; set; } = null!;
        public DbSet<CompetitorSource> CompetitorSources { get; set; } = null!;
        public DbSet<RepricingRule> RepricingRules { get; set; } = null!;
        public DbSet<RepricingSettings> RepricingSettings { get; set; } = null!;
        public DbSet<PriceChangeLog> PriceChangeLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //AdvisorGoal
            modelBuilder.Entity<AdvisorGoal>().ToTable("AdvisorGoal");
            modelBuilder.Entity<AdvisorGoal>().HasKey(o => o.Id);

            modelBuilder.Entity<AdvisorGoal>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //Prospects
            modelBuilder.Entity<Prospect>().ToTable("Prospect");
            modelBuilder.Entity<Prospect>().HasKey(x => x.Id);

            modelBuilder.Entity<Prospect>()
                .HasOne(x => x.ProspectStep)
                .WithMany()
                .HasForeignKey(x => x.ProspectStepId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prospect>()
                .HasMany(x => x.ProspectQuoteProducts)
                .WithOne(x => x.Prospect)
                .HasForeignKey(x => x.ProspectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prospect>()
                .HasOne(x => x.MetaAdCampaign)
                .WithMany()
                .HasForeignKey(x => x.MetaAdCampaignId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prospect>()
                .HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prospect>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //ProspectQuoteProduct
            modelBuilder.Entity<ProspectQuoteProduct>().ToTable("ProspectQuoteProduct");
            modelBuilder.Entity<ProspectQuoteProduct>().HasKey(x => x.Id);

            modelBuilder.Entity<ProspectQuoteProduct>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId);

            //ProspectStep
            modelBuilder.Entity<ProspectStep>().ToTable("ProspectStep");
            modelBuilder.Entity<Prospect>().HasKey(x => x.Id);

            //Country
            modelBuilder.Entity<Country>().ToTable("Country");
            modelBuilder.Entity<Country>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<Country>()
                .HasMany(p => p.Regions)
                .WithOne(x => x.Country)
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Country>()
                .Navigation(b => b.Regions)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Country>()
                .HasMany(p => p.Departments)
                .WithOne(x => x.Country)
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Country>()
                .Navigation(b => b.Departments)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            //Region
            modelBuilder.Entity<Region>().ToTable("Region");
            modelBuilder.Entity<Region>().HasKey(x => x.Id);

            modelBuilder.Entity<Region>()
                .HasMany(x => x.Departments)
                .WithOne(x => x.Region)
                .HasForeignKey(x => x.RegionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Region>()
                .Navigation(b => b.Departments)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            //OpportunitySchedules
            modelBuilder.Entity<OpportunitySchedules>().ToTable("OpportunitySchedules");
            modelBuilder.Entity<OpportunitySchedules>().HasKey(o => o.Id);

            modelBuilder.Entity<OpportunitySchedules>()
                .HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<OpportunitySchedules>(x => x.UserId);

            //AdvisorDepartment
            modelBuilder.Entity<AdvisorDepartment>().ToTable("AdvisorDepartment");
            modelBuilder.Entity<AdvisorDepartment>().HasKey(o => o.Id);

            modelBuilder.Entity<AdvisorDepartment>()
                .HasOne(x => x.User)
                .WithMany(x => x.Departments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AdvisorDepartment>()
                .HasOne(x => x.Department)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            //Brand
            modelBuilder.Entity<Brand>().ToTable("Brand");
            modelBuilder.Entity<Brand>(o => o.HasKey(x => x.Id));

            //Banner
            modelBuilder.Entity<Banner>().ToTable("Banner");
            modelBuilder.Entity<Banner>(o => o.HasKey(x => x.Id));

            //BranchOffice
            modelBuilder.Entity<BranchOffices>().ToTable("BranchOffice");
            modelBuilder.Entity<BranchOffices>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<BranchOffices>()
                .HasOne(x => x.City)
                .WithMany()
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BranchOffices>()
                .HasMany(x => x.Cais)
                .WithOne(x => x.BranchOffice)
                .HasForeignKey(x => x.BranchOfficeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BranchOffices>()
                .Navigation(b => b.Cais)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            //City
            modelBuilder.Entity<City>().ToTable("City");
            modelBuilder.Entity<City>(o => o.HasKey(x => x.Id));

            //Company
            modelBuilder.Entity<Company>().ToTable("Company");
            modelBuilder.Entity<Company>(o =>
                o.HasKey(x => x.Id));

            modelBuilder.Entity<Company>()
                .HasMany(p => p.BranchOffices)
                .WithOne(p => p.Company)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .Navigation(b => b.BranchOffices)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Company>()
                .HasMany(p => p.Banners)
                .WithOne(p => p.Company)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .Navigation(b => b.Banners)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Company>()
                .HasMany(p => p.Opinions)
                .WithOne(p => p.Company)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Company>()
                .Navigation(b => b.Opinions)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            //Currency
            modelBuilder.Entity<Currency>().ToTable("Currency");
            modelBuilder.Entity<Currency>(o => o.HasKey(x => x.Id));


            //Customer
            modelBuilder.Entity<Customer>().ToTable("Customer");
            modelBuilder.Entity<Customer>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<Customer>()
                .HasOne(x => x.CustomerType)
                .WithMany()
                .HasForeignKey(x => x.CustomerTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Customer>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Customer>()
                .HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Customer>()
                .HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Customer>()
                .HasOne(x => x.Heading)
                .WithMany()
                .HasForeignKey(x => x.HeadingId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Customer>()
                .HasOne(x => x.SocialReason)
                .WithMany()
                .HasForeignKey(x => x.SocialReasonId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Customer>()
                .HasOne(x => x.Currency)
                .WithMany()
                .HasForeignKey(x => x.CurrencyId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Customer>()
                .HasOne(x => x.Gender)
                .WithMany()
                .HasForeignKey(x => x.GenderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Customer>()
                .HasMany(x => x.DeliveryDirections)
                .WithOne(x => x.Customer)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Customer>()
                .HasMany(x => x.PendingInvoices)
                .WithOne(x => x.Customer)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Customer>()
                .HasOne(x => x.PriceList)
                .WithMany()
                .HasForeignKey(x => x.PriceListId)
                .OnDelete(DeleteBehavior.SetNull);

            //DataSheet
            modelBuilder.Entity<DataSheet>().ToTable("DataSheet");
            modelBuilder.Entity<DataSheet>(o => o.HasKey(x => x.Id));

            //Department
            modelBuilder.Entity<Department>().ToTable("Department");
            modelBuilder.Entity<Department>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<Department>()
                .HasMany(x => x.Cities)
                .WithOne(x => x.Department)
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>()
                .Navigation(b => b.Cities)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            //ExchangeRate
            modelBuilder.Entity<ExchangeRate>().ToTable("ExchangeRate");
            modelBuilder.Entity<ExchangeRate>(o => o.HasKey(x => x.Id));

            //FinancingPlan
            modelBuilder.Entity<FinancingPlan>().ToTable("FinancingPlan");
            modelBuilder.Entity<FinancingPlan>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<FinancingPlan>()
                .HasOne(x => x.Provider)
                .WithMany()
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            //Gender
            modelBuilder.Entity<Gender>().ToTable("Gender");
            modelBuilder.Entity<Gender>(o => o.HasKey(x => x.Id));

            //Message
            modelBuilder.Entity<Message>().ToTable("Message");
            modelBuilder.Entity<Message>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<Message>()
                .HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(x => x.Country)
                .WithMany()
                .HasForeignKey(x => x.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            //MetaConversations
            modelBuilder.Entity<MetaConversations>().ToTable("MetaConversations");
            modelBuilder.Entity<MetaConversations>(o => o.HasKey(x => x.Phone));

            //MetAdCampaigns
            modelBuilder.Entity<MetaAdCampaign>().ToTable("MetaAdCampaign");
            modelBuilder.Entity<MetaAdCampaign>(x => x.HasKey(y => y.Id));

            //Opinion
            modelBuilder.Entity<Opinion>().ToTable("Opinion");
            modelBuilder.Entity<Opinion>(o => o.HasKey(x => x.Id));

            //Product
            modelBuilder.Entity<Product>().ToTable("Product");
            modelBuilder.Entity<Product>(o => o.HasKey(x => x.Id));

            // Soft delete: oculta los productos eliminados de TODA consulta automaticamente.
            // Las lecturas historicas (facturas, cotizaciones, ordenes, Kardex, reportes) usan
            // Query.IgnoreQueryFilters() para seguir resolviendo el producto eliminado.
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Product>().HasIndex(p => p.IsDeleted);

            modelBuilder.Entity<Product>()
                .Property(p => p.ProductType)
                .HasConversion<int>()
                .HasDefaultValue(ProductType.Tangible);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductFeatures)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .Navigation(b => b.ProductFeatures)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductDataSheets)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasMany(a => a.InventoryDistributions)
                .WithOne(a => a.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .Navigation(b => b.ProductDataSheets)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.ProductImages)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .Navigation(b => b.ProductImages)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Product>()
                .HasOne(x => x.Brand)
                .WithMany()
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(x => x.UnitOfMeasurement)
                .WithMany()
                .HasForeignKey(x => x.UnitOfMeasurementId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(x => x.SubCategory)
                .WithMany()
                .HasForeignKey(x => x.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            //ProductSubcategory (tabla puente muchos-a-muchos Product <-> Subcategory)
            // NOTA: la tabla se llama "ProductSubcategoryLink" (no "ProductSubcategory") para evitar la
            // colisión de nombre de constraint con el PK heredado de la tabla Subcategory (PK_ProductSubCategory).
            modelBuilder.Entity<ProductSubcategory>().ToTable("ProductSubcategoryLink");
            modelBuilder.Entity<ProductSubcategory>().HasKey(x => x.Id);
            modelBuilder.Entity<ProductSubcategory>()
                .HasIndex(x => new { x.ProductId, x.SubcategoryId })
                .IsUnique();
            modelBuilder.Entity<ProductSubcategory>()
                .HasOne(x => x.Product)
                .WithMany(p => p.ProductSubcategories)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProductSubcategory>()
                .HasOne(x => x.Subcategory)
                .WithMany()
                .HasForeignKey(x => x.SubcategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(x => x.Status)
                .WithMany()
                .HasForeignKey(x => x.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Product>()
                .HasMany(x => x.ProductPurchasePriceLogs)
                .WithOne(x => x.Product)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //Category
            modelBuilder.Entity<Category>().ToTable("Category");
            modelBuilder.Entity<Category>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<Category>()
                .HasMany(p => p.HeroSliders)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .Navigation(b => b.HeroSliders)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Category>()
                .HasMany(p => p.Subcategories)
                .WithOne(x => x.Category)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .Navigation(b => b.Subcategories)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            //ProductDataSheet
            modelBuilder.Entity<ProductDataSheet>().ToTable("ProductDataSheet");
            modelBuilder.Entity<ProductDataSheet>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<ProductDataSheet>()
                .HasOne(x => x.DataSheet)
                .WithMany()
                .HasForeignKey(x => x.DataSheetId)
                .OnDelete(DeleteBehavior.Restrict);

            //ProductFeature
            modelBuilder.Entity<ProductFeature>().ToTable("ProductFeature");
            modelBuilder.Entity<ProductFeature>(o => o.HasKey(x => x.Id));

            //ProductImage
            modelBuilder.Entity<ProductImage>().ToTable("ProductImage");
            modelBuilder.Entity<ProductImage>(o => o.HasKey(x => x.Id));

            //Subcategory
            modelBuilder.Entity<Subcategory>().ToTable("Subcategory");
            modelBuilder.Entity<Subcategory>(o => o.HasKey(x => x.Id));

            //Provider
            modelBuilder.Entity<Provider>().ToTable("Provider");
            modelBuilder.Entity<Provider>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<Provider>()
                .HasOne(x => x.TypeProvider)
                .WithMany()
                .HasForeignKey(x => x.TypeProviderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Provider>()
                .HasMany<NonBillableExpense>(x=>x.NonBillableExpenses)
                .WithOne(x => x.Provider)
                .HasForeignKey(x => x.ProviderId);
            modelBuilder.Entity<Provider>()
                .HasMany<PurchaseBill>(x => x.PurchaseBills)
                .WithOne(x => x.Provider)
                .HasForeignKey(x => x.ProviderId);

            //QuoteProduct
            modelBuilder.Entity<QuoteProduct>().ToTable("QuoteProduct");
            modelBuilder.Entity<QuoteProduct>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<QuoteProduct>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //Role
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<Role>(o => o.HasKey(x => x.Id));

            //SaleOrder
            modelBuilder.Entity<SaleOrder>().ToTable("SaleOrder");
            modelBuilder.Entity<SaleOrder>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<SaleOrder>()
                .HasOne(x => x.Status)
                .WithMany()
                .HasForeignKey(x => x.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleOrder>()
                .HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleOrder>()
               .HasOne(x => x.Opportunity)
               .WithMany()
               .HasForeignKey(x => x.OpportunityId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleOrder>()
               .HasOne(x => x.FinancingPlan)
               .WithMany()
               .HasForeignKey(x => x.FinancingPlanId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleOrder>()
                .HasMany(p => p.SaleOrderProducts)
                .WithOne(x => x.SaleOrder)
                .HasForeignKey(x => x.SaleOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SaleOrder>()
                .Navigation(b => b.SaleOrderProducts)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            //SaleOrderProduct
            modelBuilder.Entity<SaleOrderProduct>().ToTable("SaleOrderProduct");
            modelBuilder.Entity<SaleOrderProduct>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<SaleOrderProduct>()
               .HasOne(x => x.Product)
               .WithMany()
               .HasForeignKey(x => x.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

            //TypeOrigin
            modelBuilder.Entity<TypeOrigin>().ToTable("TypeOrigin");
            modelBuilder.Entity<TypeOrigin>(o => o.HasKey(x => x.Id));

            //Opportunity
            modelBuilder.Entity<Opportunity>().ToTable("Opportunity");
            modelBuilder.Entity<Opportunity>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<Opportunity>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opportunity>()
                .HasOne(x => x.LossReason)
                .WithMany()
                .HasForeignKey(x => x.LossReasonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opportunity>()
                .HasOne(x => x.WinReason)
                .WithMany()
                .HasForeignKey(x => x.WinReasonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opportunity>()
                .HasOne(x => x.InterestLevel)
                .WithMany()
                .HasForeignKey(x => x.InterestLevelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opportunity>()
                .HasOne(x => x.TypeOrigin)
                .WithMany()
                .HasForeignKey(x => x.TypeOriginId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opportunity>()
                .HasOne(x => x.OpportunityStep)
                .WithMany()
                .HasForeignKey(x => x.OpportunityStepId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opportunity>()
                .HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opportunity>()
                .HasMany(p => p.QuoteProducts)
                .WithOne(x => x.Opportunity)
                .HasForeignKey(x => x.OpportunityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opportunity>()
                .Navigation(b => b.QuoteProducts)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Opportunity>()
                .HasMany(p => p.OpportunityActivities)
                .WithOne(x => x.Opportunity)
                .HasForeignKey(x => x.OpportunityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opportunity>()
                .Navigation(b => b.OpportunityActivities)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Opportunity>()
                .HasMany(p => p.OpportunityComments)
                .WithOne(x => x.Opportunity)
                .HasForeignKey(x => x.OpportunityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opportunity>()
                .Navigation(b => b.OpportunityComments)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<Opportunity>()
                .HasMany(p => p.OpportunityDocuments)
                .WithOne(x => x.Opportunity)
                .HasForeignKey(x => x.OpportunityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Opportunity>()
                .Navigation(b => b.OpportunityDocuments)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            //Status
            modelBuilder.Entity<Status>().ToTable("Status");
            modelBuilder.Entity<Status>(o => o.HasKey(x => x.Id));

            //TypeStatus
            modelBuilder.Entity<TypeStatus>().ToTable("TypeStatus");
            modelBuilder.Entity<TypeStatus>(o => o.HasKey(x => x.Id));

            //UnitOfMeasurement
            modelBuilder.Entity<UnitOfMeasurement>().ToTable("UnitOfMeasurement");
            modelBuilder.Entity<UnitOfMeasurement>(o => o.HasKey(x => x.Id));

            //User
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<User>()
               .HasOne(x => x.Gender)
               .WithMany()
               .HasForeignKey(x => x.GenderId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
               .HasOne(x => x.Role)
               .WithMany()
               .HasForeignKey(x => x.RoleId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
               .HasOne(x => x.BranchOffice)
               .WithMany()
               .HasForeignKey(x => x.BranchOfficeId)
               .OnDelete(DeleteBehavior.Restrict);


            //WishList
            modelBuilder.Entity<WishList>().ToTable("WishList");
            modelBuilder.Entity<WishList>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<WishList>()
               .HasOne(x => x.Status)
               .WithMany()
               .HasForeignKey(x => x.StatusId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WishList>()
               .HasOne(x => x.Customer)
               .WithMany()
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WishList>()
                .HasMany(p => p.WishListProducts)
                .WithOne(x => x.WishList)
                .HasForeignKey(x => x.WishListId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WishList>()
                .Navigation(b => b.WishListProducts)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            //WishListProduct
            modelBuilder.Entity<WishListProduct>().ToTable("WishListProduct");
            modelBuilder.Entity<WishListProduct>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<WishListProduct>()
               .HasOne(x => x.Product)
               .WithMany()
               .HasForeignKey(x => x.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WishListProduct>()
               .HasOne(x => x.Status)
               .WithMany()
               .HasForeignKey(x => x.StatusId)
               .OnDelete(DeleteBehavior.Restrict);

            //LogSession
            modelBuilder.Entity<LogSession>().ToTable("LogSession");
            modelBuilder.Entity<LogSession>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<LogSession>()
               .HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            //RefreshToken
            modelBuilder.Entity<RefreshToken>().ToTable("RefreshToken");
            modelBuilder.Entity<RefreshToken>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<RefreshToken>()
               .HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RefreshToken>()
               .HasIndex(x => x.TokenHash);

            //LogRecovery
            modelBuilder.Entity<LogRecovery>().ToTable("LogRecovery");
            modelBuilder.Entity<LogRecovery>(o => o.HasKey(x => x.Id));

            //HeroSlider
            modelBuilder.Entity<HeroSlider>().ToTable("HeroSlider");
            modelBuilder.Entity<HeroSlider>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<HeroSlider>()
               .HasOne(x => x.Product)
               .WithMany()
               .HasForeignKey(x => x.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

            //Notification
            modelBuilder.Entity<Notification>().ToTable("Notification");
            modelBuilder.Entity<Notification>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<Notification>()
               .HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            //DocumentType
            modelBuilder.Entity<DocumentType>().ToTable("DocumentType");
            modelBuilder.Entity<DocumentType>(o => o.HasKey(x => x.Id));

            //InterestLevel
            modelBuilder.Entity<InterestLevel>().ToTable("InterestLevel");
            modelBuilder.Entity<InterestLevel>(o => o.HasKey(x => x.Id));

            //OpportunityActivity
            modelBuilder.Entity<OpportunityActivity>().ToTable("OpportunityActivity");
            modelBuilder.Entity<OpportunityActivity>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<OpportunityActivity>()
               .HasOne(x => x.Status)
               .WithMany()
               .HasForeignKey(x => x.StatusId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OpportunityActivity>()
               .HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OpportunityActivity>()
               .HasOne(x => x.TypeActivity)
               .WithMany()
               .HasForeignKey(x => x.TypeActivityId)
               .OnDelete(DeleteBehavior.Restrict);

            //OpportunityComment
            modelBuilder.Entity<OpportunityComment>().ToTable("OpportunityComment");
            modelBuilder.Entity<OpportunityComment>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<OpportunityComment>()
               .HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            //OpportunityDocument
            modelBuilder.Entity<OpportunityDocument>().ToTable("OpportunityDocument");
            modelBuilder.Entity<OpportunityDocument>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<OpportunityDocument>()
               .HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OpportunityDocument>()
               .HasOne(x => x.DocumentType)
               .WithMany()
               .HasForeignKey(x => x.DocumentTypeId)
               .OnDelete(DeleteBehavior.Restrict);

            //OpportunityStep
            modelBuilder.Entity<OpportunityStep>().ToTable("OpportunityStep");
            modelBuilder.Entity<OpportunityStep>(o => o.HasKey(x => x.Id));

            //TypeActivity
            modelBuilder.Entity<TypeActivity>().ToTable("TypeActivity");
            modelBuilder.Entity<TypeActivity>(o => o.HasKey(x => x.Id));

            //LossReason
            modelBuilder.Entity<LossReason>().ToTable("LossReason");
            modelBuilder.Entity<LossReason>(o => o.HasKey(x => x.Id));

            //WinReason
            modelBuilder.Entity<WinReason>().ToTable("WinReason");
            modelBuilder.Entity<WinReason>(o => o.HasKey(x => x.Id));

            //CAIs
            modelBuilder.Entity<Cai>().ToTable("Cai");
            modelBuilder.Entity<Cai>(o => o.HasKey(x => x.Id));
            //InternalDocuments
            modelBuilder.Entity<InternalDocument>().ToTable("InternalDocument");
            modelBuilder.Entity<InternalDocument>(o => o.HasKey(x => x.Id));
            //Prefixes
            modelBuilder.Entity<Prefix>().ToTable("Prefix");
            modelBuilder.Entity<Prefix>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<Prefix>()
                .HasOne(x => x.InternalDocument)
                .WithMany()
                .HasForeignKey(x => x.InternalDocumentId);
            //Taxes
            modelBuilder.Entity<Tax>().ToTable("Tax");
            modelBuilder.Entity<Tax>(o => o.HasKey(x => x.Id));
            //Taxes
            modelBuilder.Entity<CustomerType>().ToTable("CustomerType");
            modelBuilder.Entity<CustomerType>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<CustomerType>()
                .HasOne(x => x.PriceList)
                .WithMany()
                .HasForeignKey(x => x.PriceListId)
                .OnDelete(DeleteBehavior.SetNull);
            //PriceList
            modelBuilder.Entity<PriceList>().ToTable("PriceList");
            modelBuilder.Entity<PriceList>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<PriceList>()
                .HasIndex(x => x.Name)
                .IsUnique();
            modelBuilder.Entity<PriceList>()
                .HasIndex(x => x.IsDefault)
                .HasFilter("[IsDefault] = 1")
                .IsUnique();
            //PriceListItem
            modelBuilder.Entity<PriceListItem>().ToTable("PriceListItem");
            modelBuilder.Entity<PriceListItem>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<PriceListItem>()
                .HasOne(x => x.PriceList)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.PriceListId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PriceListItem>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PriceListItem>()
                .HasIndex(x => new { x.PriceListId, x.ProductId })
                .IsUnique();
            modelBuilder.Entity<PriceListItem>()
                .HasIndex(x => x.ProductId);
            //Quotation
            modelBuilder.Entity<Quotation>().ToTable("Quotation");
            modelBuilder.Entity<Quotation>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<Quotation>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Quotation>()
               .HasOne(x => x.Customer)
               .WithMany()
               .HasForeignKey(x => x.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Quotation>()
               .HasOne(x => x.Status)
               .WithMany()
               .HasForeignKey(x => x.StatusId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Quotation>()
               .HasOne(x => x.BranchOffice)
               .WithMany()
               .HasForeignKey(x => x.BranchOfficeId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Quotation>()
               .HasOne(x => x.Prefix)
               .WithMany()
               .HasForeignKey(x => x.PrefixId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Quotation>()
              .HasOne(x => x.InvoiceDestination)
              .WithMany()
              .HasForeignKey(x => x.InvoiceDestinationId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Quotation>()
                .HasMany(p => p.ProductsOffered)
                .WithOne(x => x.Quotation)
                .HasForeignKey(x => x.QuotationId)
                .OnDelete(DeleteBehavior.Restrict);
            //ProductOffered
            modelBuilder.Entity<ProductOffered>().ToTable("ProductOffered");
            modelBuilder.Entity<ProductOffered>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<ProductOffered>()
                .HasOne(x => x.SourceWarehouse)
                .WithMany()
                .HasForeignKey(x => x.SourceWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            //Warehouse
            modelBuilder.Entity<Warehouse>().ToTable("Warehouse");
            modelBuilder.Entity<Warehouse>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<Warehouse>()
               .HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Warehouse>()
              .HasOne(x => x.City)
              .WithMany()
              .HasForeignKey(x => x.CityId)
              .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Warehouse>()
              .HasOne(x => x.BranchOffice)
              .WithMany()
              .HasForeignKey(x => x.BranchOfficeId)
              .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Warehouse>()
                .HasMany(a => a.InventoryDistributions)
                .WithOne(a => a.Warehouse)
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Warehouse>()
                .HasOne(x => x.WarehouseType)
                .WithMany()
                .HasForeignKey(x => x.WarehouseTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            //Inventory Distribution
            modelBuilder.Entity<InventoryDistribution>().ToTable("InventoryDistribution");
            modelBuilder.Entity<InventoryDistribution>(o => o.HasKey(x => x.Id));

            //WarehouseType
            modelBuilder.Entity<WarehouseType>().ToTable("WarehouseType");
            modelBuilder.Entity<WarehouseType>(o => o.HasKey(x => x.Id));

            //ProviderWarehouse
            modelBuilder.Entity<ProviderWarehouse>().ToTable("ProviderWarehouse");
            modelBuilder.Entity<ProviderWarehouse>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<ProviderWarehouse>()
                .HasOne(x => x.Provider)
                .WithMany(x => x.ProviderWarehouses)
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProviderWarehouse>()
                .HasOne(x => x.Warehouse)
                .WithMany(x => x.ProviderWarehouses)
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            //ShippingCostConfiguration
            modelBuilder.Entity<ShippingCostConfiguration>().ToTable("ShippingCostConfiguration");
            modelBuilder.Entity<ShippingCostConfiguration>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<ShippingCostConfiguration>()
                .HasOne(x => x.SourceWarehouse)
                .WithMany(x => x.ShippingCosts)
                .HasForeignKey(x => x.SourceWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ShippingCostConfiguration>()
                .HasOne(x => x.SourceProvider)
                .WithMany(x => x.ShippingCosts)
                .HasForeignKey(x => x.SourceProviderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ShippingCostConfiguration>()
                .HasOne(x => x.SourceCity)
                .WithMany()
                .HasForeignKey(x => x.SourceCityId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ShippingCostConfiguration>()
                .HasOne(x => x.DestinationCity)
                .WithMany()
                .HasForeignKey(x => x.DestinationCityId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ShippingCostConfiguration>()
                .HasOne(x => x.DestinationDepartment)
                .WithMany()
                .HasForeignKey(x => x.DestinationDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ShippingCostConfiguration>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //VirtualStockImport
            modelBuilder.Entity<VirtualStockImport>().ToTable("VirtualStockImport");
            modelBuilder.Entity<VirtualStockImport>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<VirtualStockImport>()
                .HasOne(x => x.Provider)
                .WithMany()
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VirtualStockImport>()
                .HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VirtualStockImport>()
                .HasMany(x => x.ImportDetails)
                .WithOne(x => x.VirtualStockImport)
                .HasForeignKey(x => x.VirtualStockImportId)
                .OnDelete(DeleteBehavior.Cascade);

            //VirtualStockImportDetail
            modelBuilder.Entity<VirtualStockImportDetail>().ToTable("VirtualStockImportDetail");
            modelBuilder.Entity<VirtualStockImportDetail>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<VirtualStockImportDetail>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //Inventory Entry
            modelBuilder.Entity<InventoryInput>().ToTable("InventoryInput");
            modelBuilder.Entity<InventoryInput>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<InventoryInput>()
               .HasOne(x => x.Warehouse)
               .WithMany()
               .HasForeignKey(x => x.WarehouseId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryInput>()
              .HasOne(x => x.PurchaseOrderOrigin)
              .WithMany()
              .HasForeignKey(x => x.PurchaseOrderOriginId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryInput>()
              .HasOne(x => x.Prefix)
              .WithMany()
              .HasForeignKey(x => x.PrefixId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryInput>()
               .HasOne(x => x.InventoryInputType)
               .WithMany()
               .HasForeignKey(x => x.InventoryInputTypeId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryInput>()
                .HasMany(p => p.ProductEntries)
                .WithOne(x => x.InventoryInput)
                .HasForeignKey(x => x.InventoryInputId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryInput>()
                .Navigation(b => b.ProductEntries)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            modelBuilder.Entity<InventoryInput>()
               .HasOne(x => x.Status)
               .WithMany()
               .HasForeignKey(x => x.StatusId)
               .OnDelete(DeleteBehavior.Restrict);
            //Inventory Input Type
            modelBuilder.Entity<InventoryInputType>().ToTable("InventoryInputType");
            modelBuilder.Entity<InventoryInputType>(o => o.HasKey(x => x.Id));
            //Product Entry
            modelBuilder.Entity<ProductEntry>().ToTable("ProductEntry");
            modelBuilder.Entity<ProductEntry>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<ProductEntry>()
              .HasOne(x => x.Product)
              .WithMany()
              .HasForeignKey(x => x.ProductId)
              .OnDelete(DeleteBehavior.Restrict);

            //InventoryEntry (Ventix-style)
            modelBuilder.Entity<InventoryEntry>().ToTable("InventoryEntry");
            modelBuilder.Entity<InventoryEntry>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<InventoryEntry>()
                .HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InventoryEntry>()
                .HasOne(x => x.Prefix)
                .WithMany()
                .HasForeignKey(x => x.PrefixId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InventoryEntry>()
                .HasOne(x => x.Provider)
                .WithMany()
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InventoryEntry>()
                .HasOne(x => x.PurchaseOrderOrigin)
                .WithMany()
                .HasForeignKey(x => x.PurchaseOrderOriginId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InventoryEntry>()
                .HasMany(x => x.Items)
                .WithOne(x => x.InventoryEntry)
                .HasForeignKey(x => x.InventoryEntryId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<InventoryEntry>()
                .HasOne(x => x.Project)
                .WithMany(p => p.InventoryEntries)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);

            //InventoryEntryItem
            modelBuilder.Entity<InventoryEntryItem>().ToTable("InventoryEntryItem");
            modelBuilder.Entity<InventoryEntryItem>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<InventoryEntryItem>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //InventoryMovement (Kardex)
            modelBuilder.Entity<InventoryMovement>().ToTable("InventoryMovement");
            modelBuilder.Entity<InventoryMovement>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<InventoryMovement>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InventoryMovement>()
                .HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InventoryMovement>()
                .HasIndex(x => new { x.ProductId, x.WarehouseId, x.MovementDate });
            modelBuilder.Entity<InventoryMovement>()
                .HasIndex(x => new { x.DocumentType, x.DocumentId });

            //InventoryExit
            modelBuilder.Entity<InventoryExit>().ToTable("InventoryExit");
            modelBuilder.Entity<InventoryExit>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<InventoryExit>()
                .HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InventoryExit>()
                .HasOne(x => x.Prefix)
                .WithMany()
                .HasForeignKey(x => x.PrefixId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InventoryExit>()
                .HasMany(x => x.Items)
                .WithOne(x => x.InventoryExit)
                .HasForeignKey(x => x.InventoryExitId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<InventoryExit>()
                .HasOne(x => x.Project)
                .WithMany(p => p.InventoryExits)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);

            //InventoryExitItem
            modelBuilder.Entity<InventoryExitItem>().ToTable("InventoryExitItem");
            modelBuilder.Entity<InventoryExitItem>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<InventoryExitItem>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //WarehouseTransfer
            modelBuilder.Entity<WarehouseTransfer>().ToTable("WarehouseTransfer");
            modelBuilder.Entity<WarehouseTransfer>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<WarehouseTransfer>()
                .HasOne(x => x.OriginWarehouse)
                .WithMany()
                .HasForeignKey(x => x.OriginWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WarehouseTransfer>()
                .HasOne(x => x.DestinationWarehouse)
                .WithMany()
                .HasForeignKey(x => x.DestinationWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WarehouseTransfer>()
                .HasOne(x => x.Prefix)
                .WithMany()
                .HasForeignKey(x => x.PrefixId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<WarehouseTransfer>()
                .HasMany(x => x.Items)
                .WithOne(x => x.WarehouseTransfer)
                .HasForeignKey(x => x.WarehouseTransferId)
                .OnDelete(DeleteBehavior.Cascade);

            //WarehouseTransferItem
            modelBuilder.Entity<WarehouseTransferItem>().ToTable("WarehouseTransferItem");
            modelBuilder.Entity<WarehouseTransferItem>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<WarehouseTransferItem>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //Invoice
            modelBuilder.Entity<Invoice>().ToTable("Invoice");
            modelBuilder.Entity<Invoice>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<Invoice>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
               .HasOne(x => x.Status)
               .WithMany()
               .HasForeignKey(x => x.StatusId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Invoice>()
               .HasOne(x => x.BranchOffice)
               .WithMany()
               .HasForeignKey(x => x.BranchOfficeId)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Invoice>()
               .HasOne(x => x.Cai)
               .WithMany()
               .HasForeignKey(x => x.CaiId)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Invoice>()
                .HasMany(p => p.ProductsSold)
                .WithOne(x => x.Invoice)
                .HasForeignKey(x => x.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Invoice>()
               .HasOne(x => x.QuotationOrigin)
               .WithMany()
               .HasForeignKey(x => x.QuotationOriginId)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Invoice>()
              .HasMany(p => p.BillPayments)
              .WithOne(x => x.Invoice)
              .HasForeignKey(x => x.InvoiceId)
              .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Invoice>()
              .HasOne(x => x.RecurringInvoiceTemplate)
              .WithMany()
              .HasForeignKey(x => x.RecurringInvoiceTemplateId)
              .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Invoice>()
              .Property(x => x.ExpectedPaymentDate)
              .HasConversion(
                  d => d.HasValue ? d.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                  d => d.HasValue ? DateOnly.FromDateTime(d.Value) : (DateOnly?)null
              );
            //Product Sold
            modelBuilder.Entity<ProductSold>().ToTable("ProductSold");
            modelBuilder.Entity<ProductSold>(o => o.HasKey(x => x.Id));
            //Bank
            modelBuilder.Entity<Bank>().ToTable("Bank");
            modelBuilder.Entity<Bank>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<Bank>()
               .HasMany(p => p.InternalBankAccounts)
               .WithOne(x => x.Bank)
               .HasForeignKey(x => x.BankId)
               .OnDelete(DeleteBehavior.Restrict);
            //Internal Bank Account
            modelBuilder.Entity<InternalBankAccount>().ToTable("InternalBankAccount");
            modelBuilder.Entity<InternalBankAccount>(o => o.HasKey(x => x.Id));
            //Bill Payment
            modelBuilder.Entity<BillPayment>().ToTable("BillPayment");
            modelBuilder.Entity<BillPayment>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<BillPayment>()
              .HasOne(x => x.TypeOfPaymentMethod)
              .WithMany()
              .HasForeignKey(x => x.TypeOfPaymentMethodId)
              .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BillPayment>()
              .HasOne(x => x.InternalBankAccount)
              .WithMany()
              .HasForeignKey(x => x.InternalBankAccountId)
              .OnDelete(DeleteBehavior.Restrict);
            //Purchase Order
            modelBuilder.Entity<PurchaseOrder>().ToTable("PurchaseOrder");
            modelBuilder.Entity<PurchaseOrder>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
               .HasOne(x => x.Provider)
               .WithMany()
               .HasForeignKey(x => x.ProviderId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
               .HasOne(x => x.Status)
               .WithMany()
               .HasForeignKey(x => x.StatusId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
               .HasOne(x => x.BranchOffice)
               .WithMany()
               .HasForeignKey(x => x.BranchOfficeId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
               .HasOne(x => x.Prefix)
               .WithMany()
               .HasForeignKey(x => x.PrefixId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
              .HasOne(x => x.PurchaseBillDestination)
              .WithMany()
              .HasForeignKey(x => x.PurchaseBillDestinationId)
              .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(x => x.InventoryInputDestination)
                .WithMany()
                .HasForeignKey(x => x.InventoryInputDestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(x => x.InventoryEntryDestination)
                .WithMany()
                .HasForeignKey(x => x.InventoryEntryDestinationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasMany(p => p.ProductsToPurchase)
                .WithOne(x => x.PurchaseOrder)
                .HasForeignKey(x => x.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);
            //Product To Purchase
            modelBuilder.Entity<ProductToPurchase>().ToTable("ProductToPurchase");
            modelBuilder.Entity<ProductToPurchase>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<ProductToPurchase>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            //Type Provider
            modelBuilder.Entity<TypeProvider>().ToTable("TypeProvider");
            modelBuilder.Entity<TypeProvider>(o => o.HasKey(x => x.Id));

            //Purchase Bill
            modelBuilder.Entity<PurchaseBill>().ToTable("PurchaseBill");
            modelBuilder.Entity<PurchaseBill>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<PurchaseBill>()
               .HasOne(x => x.Status)
               .WithMany()
               .HasForeignKey(x => x.StatusId)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PurchaseBill>()
               .HasOne(x => x.PurchaseOrderOrigin)
               .WithMany()
               .HasForeignKey(x => x.PurchaseOrderOriginId)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PurchaseBill>()
              .HasMany(p => p.PurchaseBillPayments)
              .WithOne(x => x.PurchaseBill)
              .HasForeignKey(x => x.PurchaseBillId)
              .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PurchaseBill>()
               .HasOne(x => x.Prefix)
               .WithMany()
               .HasForeignKey(x => x.PrefixId)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PurchaseBill>()
               .HasOne(x => x.ExpenseAccount)
               .WithMany()
               .HasForeignKey(x => x.ExpenseAccountId)
               .OnDelete(DeleteBehavior.Restrict);
            //Purchase Bill Payment
            modelBuilder.Entity<PurchaseBillPayment>().ToTable("PurchaseBillPayment");
            modelBuilder.Entity<PurchaseBillPayment>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<PurchaseBillPayment>()
              .HasOne(x => x.TypeOfPaymentMethod)
              .WithMany()
              .HasForeignKey(x => x.TypeOfPaymentMethodId)
              .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PurchaseBillPayment>()
              .HasOne(x => x.InternalBankAccount)
              .WithMany()
              .HasForeignKey(x => x.InternalBankAccountId)
              .OnDelete(DeleteBehavior.Restrict);
            //Major Expense Account
            modelBuilder.Entity<MajorExpenseAccount>().ToTable("MajorExpenseAccount");
            modelBuilder.Entity<MajorExpenseAccount>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<MajorExpenseAccount>()
             .HasMany(p => p.ExpenseAccounts)
             .WithOne(x => x.MajorExpenseAccount)
             .HasForeignKey(x => x.MajorExpenseAccountId)
             .OnDelete(DeleteBehavior.Restrict);
            //Expense Account
            modelBuilder.Entity<ExpenseAccount>().ToTable("ExpenseAccount");
            modelBuilder.Entity<ExpenseAccount>(o => o.HasKey(x => x.Id));
            //Major Income Account
            modelBuilder.Entity<MajorIncomeAccount>().ToTable("MajorIncomeAccount");
            modelBuilder.Entity<MajorIncomeAccount>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<MajorIncomeAccount>()
             .HasMany(p => p.IncomeAccounts)
             .WithOne(x => x.MajorIncomeAccount)
             .HasForeignKey(x => x.MajorIncomeAccountId)
             .OnDelete(DeleteBehavior.Restrict);
            //Income Account
            modelBuilder.Entity<IncomeAccount>().ToTable("IncomeAccount");
            modelBuilder.Entity<IncomeAccount>(o => o.HasKey(x => x.Id));
            //Product Purchase Price Log
            modelBuilder.Entity<ProductPurchasePriceLog>().ToTable("ProductPurchasePriceLog");
            modelBuilder.Entity<ProductPurchasePriceLog>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<ProductPurchasePriceLog>()
             .HasOne(x => x.PurchaseBillOrigin)
             .WithMany()
             .HasForeignKey(x => x.PurchaseBillOriginId)
             .OnDelete(DeleteBehavior.Restrict);
            //NonBillable Expense
            modelBuilder.Entity<NonBillableExpense>().ToTable("NonBillableExpense");
            modelBuilder.Entity<NonBillableExpense>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<NonBillableExpense>()
            .HasOne(x => x.Prefix)
            .WithMany()
            .HasForeignKey(x => x.PrefixId)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<NonBillableExpense>()
            .HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<NonBillableExpense>()
            .HasOne(x => x.ExpenseAccount)
            .WithMany()
            .HasForeignKey(x => x.ExpenseAccountId)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<NonBillableExpense>()
             .HasMany(p => p.NonBillableExpensePayments)
             .WithOne(x => x.NonBillableExpense)
             .HasForeignKey(x => x.NonBillableExpenseId)
             .OnDelete(DeleteBehavior.Restrict);
            //NonBillable Expense Payment
            modelBuilder.Entity<NonBillableExpensePayment>().ToTable("NonBillableExpensePayment");
            modelBuilder.Entity<NonBillableExpensePayment>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<NonBillableExpensePayment>()
             .HasOne(x => x.TypeOfPaymentMethod)
             .WithMany()
             .HasForeignKey(x => x.TypeOfPaymentMethodId)
             .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<NonBillableExpensePayment>()
            .HasOne(x => x.InternalBankAccount)
            .WithMany()
            .HasForeignKey(x => x.InternalBankAccountId)
            .OnDelete(DeleteBehavior.Restrict);
            //Credit Card Payment — pago de TC desde una cuenta bancaria (ambas son InternalBankAccount).
            modelBuilder.Entity<CreditCardPayment>().ToTable("CreditCardPayment");
            modelBuilder.Entity<CreditCardPayment>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<CreditCardPayment>()
             .HasIndex(x => x.Code).IsUnique();
            modelBuilder.Entity<CreditCardPayment>()
             .HasIndex(x => x.Date);
            modelBuilder.Entity<CreditCardPayment>()
             .HasOne(x => x.CreditCardInternalBankAccount)
             .WithMany()
             .HasForeignKey(x => x.CreditCardInternalBankAccountId)
             .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CreditCardPayment>()
             .HasOne(x => x.SourceInternalBankAccount)
             .WithMany()
             .HasForeignKey(x => x.SourceInternalBankAccountId)
             .OnDelete(DeleteBehavior.Restrict);
            //Daily Close
            modelBuilder.Entity<DailyClose>().ToTable("DailyClose");
            modelBuilder.Entity<DailyClose>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<DailyClose>()
             .HasOne(x => x.BranchOffice)
             .WithMany()
             .HasForeignKey(x => x.BranchOfficeId)
             .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DailyClose>()
            .HasOne(x => x.Cai)
            .WithMany()
            .HasForeignKey(x => x.CaiId)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<DailyClose>()
             .HasMany(p => p.ResumePayments)
             .WithOne(x => x.DailyClose)
             .HasForeignKey(x => x.DailyCloseId)
             .OnDelete(DeleteBehavior.Cascade);
            //Resume Payment
            modelBuilder.Entity<ResumePayment>().ToTable("ResumePayment");
            modelBuilder.Entity<ResumePayment>(o => o.HasKey(x => x.Id));
            // Monthly Purchase Declaration
            modelBuilder.Entity<MonthlyPurchaseDeclaration>().ToTable("MonthlyPurchaseDeclaration");
            modelBuilder.Entity<MonthlyPurchaseDeclaration>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<MonthlyPurchaseDeclaration>()
             .HasMany(x => x.DeclaratedPurchaseBills)
             .WithOne(x => x.MonthlyPurchaseDeclaration)
             .HasForeignKey(x => x.MonthlyPurchaseDeclarationId)
             .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<MonthlyPurchaseDeclaration>()
             .HasOne(x => x.Status)
             .WithMany()
             .HasForeignKey(x => x.StatusId)
             .OnDelete(DeleteBehavior.Restrict);
            //Declarated Purchase Bill
            modelBuilder.Entity<DeclaratedPurchaseBill>().ToTable("DeclaratedPurchaseBill");
            modelBuilder.Entity<DeclaratedPurchaseBill>(o => o.HasKey(x => x.Id));
            // Monthly Sale Declaration
            modelBuilder.Entity<MonthlySaleDeclaration>().ToTable("MonthlySaleDeclaration");
            modelBuilder.Entity<MonthlySaleDeclaration>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<MonthlySaleDeclaration>()
             .HasMany(x => x.DeclaredSaleInvoices)
             .WithOne(x => x.MonthlySaleDeclaration)
             .HasForeignKey(x => x.MonthlySaleDeclarationId)
             .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<MonthlySaleDeclaration>()
             .HasOne(x => x.Status)
             .WithMany()
             .HasForeignKey(x => x.StatusId)
             .OnDelete(DeleteBehavior.Restrict);
            //Declared Sale Invoice
            modelBuilder.Entity<DeclaredSaleInvoice>().ToTable("DeclaredSaleInvoice");
            modelBuilder.Entity<DeclaredSaleInvoice>(o => o.HasKey(x => x.Id));
            //Invoice Payment Type
            modelBuilder.Entity<InvoicePaymentType>().ToTable("InvoicePaymentType");
            modelBuilder.Entity<InvoicePaymentType>(o => o.HasKey(x => x.Id));
            //Discount
            modelBuilder.Entity<Discount>().ToTable("Discount");
            modelBuilder.Entity<Discount>(o => o.HasKey(x => x.Id));
            //Ecommerce User
            modelBuilder.Entity<EcommerceUser>().ToTable("EcommerceUser");
            modelBuilder.Entity<EcommerceUser>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<EcommerceUser>()
                .HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey(x => x.DepartmentId);
            modelBuilder.Entity<EcommerceUser>()
                .HasOne(x => x.Gender)
                .WithMany()
                .HasForeignKey(x => x.GenderId);
            modelBuilder.Entity<EcommerceUser>()
                .HasOne(x => x.CustomerType)
                .WithMany()
                .HasForeignKey(x => x.CustomerTypeId);
            modelBuilder.Entity<EcommerceUser>()
                .HasMany(x=>x.Carts)
                .WithOne(y=>y.EcommerceUser)
                .HasForeignKey(y=>y.EcommerceUserId)
                .OnDelete(DeleteBehavior.Cascade);
            //LogEcommerceUser
            modelBuilder.Entity<LogEcommerceUser>().ToTable("LogEcommerceUser");
            modelBuilder.Entity<LogEcommerceUser>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<LogEcommerceUser>()
                .HasOne(x => x.EcommerceUser)
                .WithMany()
                .HasForeignKey(x => x.EcommerceUserId)
                .OnDelete(DeleteBehavior.Restrict);
            //AssociatedCompany
            modelBuilder.Entity<AssociatedCompany>().ToTable("AssociatedCompany");
            modelBuilder.Entity<AssociatedCompany>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<AssociatedCompany>()
                .HasOne(x => x.EcommerceUser)
                .WithMany()
                .HasForeignKey(x => x.EcommerceUserId)
                .OnDelete(DeleteBehavior.Cascade);
            //PaymentMethod
            modelBuilder.Entity<PaymentMethod>().ToTable("PaymentMethod");
            modelBuilder.Entity<PaymentMethod>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<PaymentMethod>()
                .HasOne(x => x.EcommerceUser)
                .WithMany()
                .HasForeignKey(x => x.EcommerceUserId)
                .OnDelete(DeleteBehavior.Cascade);
            //Cart
            modelBuilder.Entity<Cart>().ToTable("Cart");
            modelBuilder.Entity<Cart>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<Cart>()
                .Property(x => x.Status)
                .HasDefaultValue(CartStatus.Active);
            modelBuilder.Entity<Cart>()
                .Property(x => x.PaymentLinkUrl)
                .HasMaxLength(500);
            modelBuilder.Entity<Cart>()
                .HasMany(p => p.CartItems)
                .WithOne(x => x.Cart)
                .HasForeignKey(x => x.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            //Cart Item
            modelBuilder.Entity<CartItem>().ToTable("CartItem");
            modelBuilder.Entity<CartItem>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<CartItem>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId);
            modelBuilder.Entity<CartItem>()
                .Property(x=>x.TotalPrice)
                .HasComputedColumnSql("[Quantity] * [UnitPrice] - ISNULL([Discount], 0)");
            modelBuilder.Entity<CartItem>()
                .Property(x => x.ProductDescription)
                .HasMaxLength(500);

            //Product Parts
            modelBuilder.Entity<ProductPart>().ToTable("ProductParts");
            modelBuilder.Entity<ProductPart>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<ProductPart>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProductPart>()
                .HasOne(x => x.FatherProduct)
                .WithMany(p => p.Components)
                .HasForeignKey(x => x.FatherProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ProductPart>()
                .HasIndex(x => new { x.FatherProductId, x.IsActive });

            //ProjectAttachmentCategory
            modelBuilder.Entity<ProjectAttachmentCategory>().ToTable("ProjectAttachmentCategory");
            modelBuilder.Entity<ProjectAttachmentCategory>(o => o.HasKey(x => x.Id));

            //ProjectAttachment
            modelBuilder.Entity<ProjectAttachment>().ToTable("ProjectAttachment");
            modelBuilder.Entity<ProjectAttachment>(o => o.HasKey(x => x.Id));

            modelBuilder.Entity<ProjectAttachment>()
               .HasOne(x => x.User)
               .WithMany()
               .HasForeignKey(x => x.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectAttachment>()
               .HasOne(x => x.ProjectAttachmentCategory)
               .WithMany()
               .HasForeignKey(x => x.ProjectAttachmentCategoryId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.ProjectAttachments)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .Navigation(b => b.ProjectAttachments)
                .UsePropertyAccessMode(PropertyAccessMode.Property);

            //ChatSession
            modelBuilder.Entity<ChatSession>().ToTable("ChatSession");
            modelBuilder.Entity<ChatSession>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<ChatSession>()
                .HasOne(x => x.EcommerceUser)
                .WithMany()
                .HasForeignKey(x => x.EcommerceUserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChatSession>()
                .HasOne(x => x.AssignedAdminUser)
                .WithMany()
                .HasForeignKey(x => x.AssignedAdminUserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ChatSession>()
                .HasMany(x => x.Messages)
                .WithOne(x => x.ChatSession)
                .HasForeignKey(x => x.ChatSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            //ChatMessage
            modelBuilder.Entity<ChatMessage>().ToTable("ChatMessage");
            modelBuilder.Entity<ChatMessage>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<ChatMessage>()
                .HasOne(x => x.SenderAdminUser)
                .WithMany()
                .HasForeignKey(x => x.SenderAdminUserId)
                .OnDelete(DeleteBehavior.Restrict);

            //RecurringInvoiceTemplate
            modelBuilder.Entity<RecurringInvoiceTemplate>().ToTable("RecurringInvoiceTemplate");
            modelBuilder.Entity<RecurringInvoiceTemplate>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<RecurringInvoiceTemplate>()
                .HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RecurringInvoiceTemplate>()
                .HasOne(x => x.BranchOffice)
                .WithMany()
                .HasForeignKey(x => x.BranchOfficeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RecurringInvoiceTemplate>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RecurringInvoiceTemplate>()
                .HasOne(x => x.InvoicePaymentType)
                .WithMany()
                .HasForeignKey(x => x.InvoicePaymentTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RecurringInvoiceTemplate>()
                .HasOne(x => x.Status)
                .WithMany()
                .HasForeignKey(x => x.StatusId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RecurringInvoiceTemplate>()
                .HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RecurringInvoiceTemplate>()
                .HasMany(x => x.Items)
                .WithOne(x => x.RecurringInvoiceTemplate)
                .HasForeignKey(x => x.RecurringInvoiceTemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            //RecurringInvoiceTemplateItem
            modelBuilder.Entity<RecurringInvoiceTemplateItem>().ToTable("RecurringInvoiceTemplateItem");
            modelBuilder.Entity<RecurringInvoiceTemplateItem>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<RecurringInvoiceTemplateItem>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RecurringInvoiceTemplateItem>()
                .HasOne(x => x.Tax)
                .WithMany()
                .HasForeignKey(x => x.TaxId)
                .OnDelete(DeleteBehavior.Restrict);

            //RecurringInvoiceLog
            modelBuilder.Entity<RecurringInvoiceLog>().ToTable("RecurringInvoiceLog");
            modelBuilder.Entity<RecurringInvoiceLog>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<RecurringInvoiceLog>()
                .HasOne(x => x.RecurringInvoiceTemplate)
                .WithMany()
                .HasForeignKey(x => x.RecurringInvoiceTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RecurringInvoiceLog>()
                .HasOne(x => x.GeneratedInvoice)
                .WithMany()
                .HasForeignKey(x => x.GeneratedInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            //QuotationSnapshot
            modelBuilder.Entity<QuotationSnapshot>().ToTable("QuotationSnapshot");
            modelBuilder.Entity<QuotationSnapshot>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<QuotationSnapshot>()
                .HasOne(x => x.Quotation)
                .WithMany()
                .HasForeignKey(x => x.QuotationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<QuotationSnapshot>()
                .HasIndex(x => new { x.QuotationId, x.VersionNumber });

            //Quotation AccessToken unique filtered index
            modelBuilder.Entity<Quotation>()
                .HasIndex(x => x.AccessToken)
                .IsUnique()
                .HasFilter("[AccessToken] IS NOT NULL");

            //QuotationComment
            modelBuilder.Entity<QuotationComment>().ToTable("QuotationComment");
            modelBuilder.Entity<QuotationComment>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<QuotationComment>()
                .HasOne(x => x.Quotation)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.QuotationId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<QuotationComment>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //QuotationItemObservation
            modelBuilder.Entity<QuotationItemObservation>().ToTable("QuotationItemObservation");
            modelBuilder.Entity<QuotationItemObservation>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<QuotationItemObservation>()
                .HasOne(x => x.Quotation)
                .WithMany(x => x.ItemObservations)
                .HasForeignKey(x => x.QuotationId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<QuotationItemObservation>()
                .HasOne(x => x.ProductOffered)
                .WithMany()
                .HasForeignKey(x => x.ProductOfferedId)
                .OnDelete(DeleteBehavior.Restrict);

            //=================== MÓDULO CONTABLE (Partida Doble) ===================

            //LedgerAccount (Catálogo de Cuentas)
            modelBuilder.Entity<LedgerAccount>().ToTable("LedgerAccount");
            modelBuilder.Entity<LedgerAccount>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<LedgerAccount>()
                .HasIndex(x => x.Code)
                .IsUnique();
            modelBuilder.Entity<LedgerAccount>()
                .HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<LedgerAccount>()
                .HasOne(x => x.ExpenseAccount)
                .WithMany()
                .HasForeignKey(x => x.ExpenseAccountId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<LedgerAccount>()
                .HasOne(x => x.IncomeAccount)
                .WithMany()
                .HasForeignKey(x => x.IncomeAccountId)
                .OnDelete(DeleteBehavior.SetNull);

            //FiscalYear
            modelBuilder.Entity<FiscalYear>().ToTable("FiscalYear");
            modelBuilder.Entity<FiscalYear>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<FiscalYear>()
                .HasIndex(x => x.Year)
                .IsUnique();
            modelBuilder.Entity<FiscalYear>()
                .HasMany(x => x.Periods)
                .WithOne(x => x.FiscalYear)
                .HasForeignKey(x => x.FiscalYearId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<FiscalYear>()
                .HasOne(x => x.ClosingJournalEntry)
                .WithMany()
                .HasForeignKey(x => x.ClosingJournalEntryId)
                .OnDelete(DeleteBehavior.Restrict);

            //FiscalPeriod
            modelBuilder.Entity<FiscalPeriod>().ToTable("FiscalPeriod");
            modelBuilder.Entity<FiscalPeriod>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<FiscalPeriod>()
                .HasIndex(x => new { x.FiscalYearId, x.PeriodNumber })
                .IsUnique();

            //JournalEntry (Asiento)
            modelBuilder.Entity<JournalEntry>().ToTable("JournalEntry");
            modelBuilder.Entity<JournalEntry>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<JournalEntry>()
                .HasIndex(x => x.EntryNumber)
                .IsUnique()
                .HasFilter("[EntryNumber] IS NOT NULL");
            modelBuilder.Entity<JournalEntry>()
                .HasIndex(x => x.EntryDate);
            modelBuilder.Entity<JournalEntry>()
                .HasIndex(x => x.Status);
            modelBuilder.Entity<JournalEntry>()
                .HasOne(x => x.FiscalPeriod)
                .WithMany()
                .HasForeignKey(x => x.FiscalPeriodId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<JournalEntry>()
                .HasOne(x => x.ReversesJournalEntry)
                .WithMany()
                .HasForeignKey(x => x.ReversesJournalEntryId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<JournalEntry>()
                .HasOne(x => x.ReversedByJournalEntry)
                .WithMany()
                .HasForeignKey(x => x.ReversedByJournalEntryId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<JournalEntry>()
                .HasMany(x => x.Lines)
                .WithOne(x => x.JournalEntry)
                .HasForeignKey(x => x.JournalEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            //JournalEntryLine (Partida)
            modelBuilder.Entity<JournalEntryLine>().ToTable("JournalEntryLine");
            modelBuilder.Entity<JournalEntryLine>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<JournalEntryLine>()
                .HasIndex(x => x.LedgerAccountId);
            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(x => x.LedgerAccount)
                .WithMany()
                .HasForeignKey(x => x.LedgerAccountId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            //AccountingSettings (fila única)
            modelBuilder.Entity<AccountingSettings>().ToTable("AccountingSettings");
            modelBuilder.Entity<AccountingSettings>(o => o.HasKey(x => x.Id));

            //AccountingMapping
            modelBuilder.Entity<AccountingMapping>().ToTable("AccountingMapping");
            modelBuilder.Entity<AccountingMapping>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<AccountingMapping>().HasIndex(x => x.Key).IsUnique();
            modelBuilder.Entity<AccountingMapping>()
                .HasOne(x => x.LedgerAccount)
                .WithMany()
                .HasForeignKey(x => x.LedgerAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            //InternalBankAccount -> LedgerAccount (mapeo contable)
            modelBuilder.Entity<InternalBankAccount>()
                .HasOne(x => x.LedgerAccount)
                .WithMany()
                .HasForeignKey(x => x.LedgerAccountId)
                .OnDelete(DeleteBehavior.SetNull);

            //CostCenter (dimensión: línea de negocio / centro de costo)
            modelBuilder.Entity<CostCenter>().ToTable("CostCenter");
            modelBuilder.Entity<CostCenter>(o => o.HasKey(x => x.Id));
            modelBuilder.Entity<CostCenter>().HasIndex(x => x.Code).IsUnique();

            //JournalEntryLine — dimensiones añadidas: CostCenter + Tercero (Customer/Provider)
            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(x => x.CostCenter)
                .WithMany()
                .HasForeignKey(x => x.CostCenterId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<JournalEntryLine>()
                .HasOne(x => x.Provider)
                .WithMany()
                .HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            //Repricing — monitoreo de precios de competencia + re-fijación automática
            modelBuilder.Entity<CompetitorSource>().ToTable("CompetitorSource");
            modelBuilder.Entity<CompetitorSource>().HasKey(x => x.Id);
            modelBuilder.Entity<CompetitorSource>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CompetitorSource>()
                .HasIndex(x => new { x.ProductId, x.CompetitorName }).IsUnique();

            modelBuilder.Entity<RepricingRule>().ToTable("RepricingRule");
            modelBuilder.Entity<RepricingRule>().HasKey(x => x.Id);
            modelBuilder.Entity<RepricingRule>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RepricingRule>()
                .HasIndex(x => x.ProductId).IsUnique();

            modelBuilder.Entity<RepricingSettings>().ToTable("RepricingSettings");
            modelBuilder.Entity<RepricingSettings>().HasKey(x => x.Id);

            modelBuilder.Entity<PriceChangeLog>().ToTable("PriceChangeLog");
            modelBuilder.Entity<PriceChangeLog>().HasKey(x => x.Id);
            modelBuilder.Entity<PriceChangeLog>()
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PriceChangeLog>()
                .HasIndex(x => new { x.ProductId, x.CreatedUtc });

            base.OnModelCreating(modelBuilder);
        }
    }
}