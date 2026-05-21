using AutoMapper;
using SMART.ERP.Application.DTOs.Address;
using SMART.ERP.Application.DTOs.AdvisorDepartment;
using SMART.ERP.Application.DTOs.AssociatedCompany;
using SMART.ERP.Application.DTOs.PaymentMethod;
using SMART.ERP.Application.DTOs.AdvisorGoal;
using SMART.ERP.Application.DTOs.Bank;
using SMART.ERP.Application.DTOs.BillPayment;
using SMART.ERP.Application.DTOs.Cai;
using SMART.ERP.Application.DTOs.Cart;
using SMART.ERP.Application.DTOs.CartItem;
using SMART.ERP.Application.DTOs.Chat;
using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.DTOs.DailyClose;
using SMART.ERP.Application.DTOs.Dashboard;
using SMART.ERP.Application.DTOs.DeclaratedPurchaseBill;
using SMART.ERP.Application.DTOs.DeclaredSaleInvoice;
using SMART.ERP.Application.DTOs.Discount;
using SMART.ERP.Application.DTOs.EcommerceUser;
using SMART.ERP.Application.DTOs.LogEcommerceUser;
using SMART.ERP.Application.DTOs.ExpenseAccount;
using SMART.ERP.Application.DTOs.IncomeAccount;
using SMART.ERP.Application.DTOs.InternalBankAccount;
using SMART.ERP.Application.DTOs.InventoryDistribution;
using SMART.ERP.Application.DTOs.InventoryInput;
using SMART.ERP.Application.DTOs.InventoryInputType;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.DTOs.InvoicePaymentType;
using SMART.ERP.Application.DTOs.RecurringInvoiceTemplate;
using SMART.ERP.Application.DTOs.MajorExpenseAccount;
using SMART.ERP.Application.DTOs.MajorIncomeAccount;
using SMART.ERP.Application.DTOs.Meta.MetAdCampaign;
using SMART.ERP.Application.DTOs.MonthlyPurchaseDeclaration;
using SMART.ERP.Application.DTOs.MonthlySaleDeclaration;
using SMART.ERP.Application.DTOs.NonBillableExpensePayment;
using SMART.ERP.Application.DTOs.NonBilllableExpense;
using SMART.ERP.Application.DTOs.Notification;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.DTOs.PriceList;
using SMART.ERP.Application.DTOs.Product;
using SMART.ERP.Application.DTOs.Project;
using SMART.ERP.Application.DTOs.ProjectAttachment;
using SMART.ERP.Application.DTOs.ProjectAttachmentCategory;
using SMART.ERP.Application.DTOs.ProductEntry;
using SMART.ERP.Application.DTOs.ProductPurchasePriceLog;
using SMART.ERP.Application.DTOs.ProductToPurchase;
using SMART.ERP.Application.DTOs.Prospect;
using SMART.ERP.Application.DTOs.ProspectQuoteProduct;
using SMART.ERP.Application.DTOs.Provider;
using SMART.ERP.Application.DTOs.ProviderWarehouse;
using SMART.ERP.Application.DTOs.PurchaseBill;
using SMART.ERP.Application.DTOs.PurchaseBillPayment;
using SMART.ERP.Application.DTOs.PurchaseOrder;
using SMART.ERP.Application.DTOs.Quotation;
using SMART.ERP.Application.DTOs.ResumePayment;
using SMART.ERP.Application.DTOs.SaleOrder;
using SMART.ERP.Application.DTOs.ShippingCost;
using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.DTOs.TypeOfPaymentMethod;
using SMART.ERP.Application.DTOs.TypeProvider;
using SMART.ERP.Application.DTOs.User;
using SMART.ERP.Application.DTOs.Warehouse;
using SMART.ERP.Application.DTOs.WishList;
using SMART.ERP.Application.Features.AssociatedCompanyFeature.Commands.CreateAssociatedCompanyCommand;
using SMART.ERP.Application.Features.PaymentMethodFeature.Commands.CreatePaymentMethodCommand;
using SMART.ERP.Application.Features.BankFeature.Commands.CreateBankCommand;
using SMART.ERP.Application.Features.BannerFeature.Commands.CreateBannerCommand;
using SMART.ERP.Application.Features.BaseProductFeature.Commands.CreateBaseProductCommand;
using SMART.ERP.Application.Features.BillPaymentFeature.Commands.CreateBillPaymentCommand;
using SMART.ERP.Application.Features.BranchOfficeFeature.Commands.CreateBranchOfficeCommand;
using SMART.ERP.Application.Features.BrandFeature.Commands.CreateBrandCommand;
using SMART.ERP.Application.Features.CaiFeature.Commands.CreateCaiCommand;
using SMART.ERP.Application.Features.CategoryFeature.Commands.CreateCategoryCommand;
using SMART.ERP.Application.Features.CityFeature.Commands.CreateCityCommand;
using SMART.ERP.Application.Features.ClientDeliveryDirectionFeature.Commands.CreateDeliveryDirectionCommand;
using SMART.ERP.Application.Features.CompanyFeature.Commands.CreateCompanyCommand;
using SMART.ERP.Application.Features.CountryFeature.Commands.CreateCountryCommand;
using SMART.ERP.Application.Features.CustomerFeature.Commands.CreateCustomerCommand;
using SMART.ERP.Application.Features.DailyClosinFeature.Commands.CreateDailyCloseCommand;
using SMART.ERP.Application.Features.DataSheetFeature.Commands.CreateDataSheetCommand;
using SMART.ERP.Application.Features.DepartmentFeature.Commands.CreateDepartmentCommand;
using SMART.ERP.Application.Features.DiscountFeature.Commands.CreateDiscountCommand;
using SMART.ERP.Application.Features.DocumentTypeFeature.Commands.CreateDocumentTypeCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.AdminCreateEcommerceUserCommand;
using SMART.ERP.Application.Features.EcommerceUserFeature.Commands.CreateEcommerceUserCommand;
using SMART.ERP.Application.Features.ExpenseAccountFeature.Commands.CreateExpenseAccountCommand;
using SMART.ERP.Application.Features.FinancingPlanFeature.Commands.CreateFinancingPlanCommand;
using SMART.ERP.Application.Features.HeroSliderFeature.Commands.CreateHeroSliderCommand;
using SMART.ERP.Application.Features.IncomeAccountFeature.Commands.CreateIncomeAccountCommand;
using SMART.ERP.Application.Features.InterestLevelFeature.Commands.CreateInterestLevelCommand;
using SMART.ERP.Application.Features.InternalBankAccountFeature.Commands.CreateInternalBankAccountCommand;
using SMART.ERP.Application.Features.InventoryInputFeature.Commands.CreateInventoryInputCommand;
using SMART.ERP.Application.Features.InventoryInputFeature.Commands.CreateInventoryInputCommandByPurchaseOrderIdCommand;
using SMART.ERP.Application.Features.InventoryInputTypeFeature.Commands.CreateInventoryInputTypeCommand;
using SMART.ERP.Application.Features.InvoiceFeature.Commands.CancelInvoiceCommand;
using SMART.ERP.Application.Features.InvoiceFeature.Commands.CreateInvoiceCommand;
using SMART.ERP.Application.Features.InvoiceFeature.Commands.CreateInvoiceFromPosScreenCommand;
using SMART.ERP.Application.Features.LossReasonFeature.Commands.CreateLossReasonCommand;
using SMART.ERP.Application.Features.MajorExpenseAccountFeature.Commands.CreateMajorExpenseAccountCommand;
using SMART.ERP.Application.Features.MajorIncomeAccountFeature.Commands.CreateMajorIncomeAccountCommand;
using SMART.ERP.Application.Features.MessageFeature.Commands.CreateMessageCommand;
using SMART.ERP.Application.Features.NonBillableExpenseFeature.Commands.CreateNonBillableExpenseCommand;
using SMART.ERP.Application.Features.NonBillableExpensePaymentFeature.Commands.CreateNonBillableExpensePaymentCommand;
using SMART.ERP.Application.Features.OpinionFeature.Commands.CreateOpinionCommand;
using SMART.ERP.Application.Features.OpportunityActivityFeature.Commands.CreateOpportunityActivityCommand;
using SMART.ERP.Application.Features.OpportunityCommentFeature.Commands.CreateOpportunityCommentCommand;
using SMART.ERP.Application.Features.OpportunityDocumentFeature.Commands.CreateOpportunityDocumentCommand;
using SMART.ERP.Application.Features.OpportunityFeature.Commands.CreateOpportunityCommand;
using SMART.ERP.Application.Features.PrefixFeature.Command.CreatePrefixCommand;
using SMART.ERP.Application.Features.ProductDataSheetFeature.Commands.CreateProductDataSheetCommand;
using SMART.ERP.Application.Features.ProjectAttachmentCategoryFeature.Commands.CreateProjectAttachmentCategoryCommand;
using SMART.ERP.Application.Features.ProjectAttachmentFeature.Commands.CreateProjectAttachmentCommand;
using SMART.ERP.Application.Features.ProjectFeature.Commands.CreateProjectCommand;
using SMART.ERP.Application.Features.ProductFtrFeature.Commands.CreateProductFtrCommand;
using SMART.ERP.Application.Features.ProductImageFeature.Commands.CreateProductImageCommand;
using SMART.ERP.Application.Features.ProviderFeature.Commands.CreateProviderCommand;
using SMART.ERP.Application.Features.PurchaseBillFeature.Commands.CreatePurchaseBillCommand;
using SMART.ERP.Application.Features.PurchaseBillFeature.Commands.CreatePurchaseBillFromPurchaseOrderDetailPageCommand;
using SMART.ERP.Application.Features.PurchaseBillPaymentFeature.Commands.CreatePurchaseBillPaymentCommand;
using SMART.ERP.Application.Features.PurchaseOrderFeature.Commands.CreatePurchaseOrderCommand;
using SMART.ERP.Application.Features.QuotationFeature.Commands.CopyQuotationFromIdCommand;
using SMART.ERP.Application.Features.QuotationFeature.Commands.CreateQuotationCommand;
using SMART.ERP.Application.Features.QuoteProductFeature.Commands.CreateQuoteProductCommand;
using SMART.ERP.Application.Features.RegionFeature.Commands.CreateRegionCommand;
using SMART.ERP.Application.Features.StatusFeature.Commands.CreateStatusCommand;
using SMART.ERP.Application.Features.SubcategoryFeature.Commands.CreateSubcategoryCommand;
using SMART.ERP.Application.Features.TaxFeature.Commands.CreateTaxCommand;
using SMART.ERP.Application.Features.TypeActivityFeature.Commands.CreateTypeActivityCommand;
using SMART.ERP.Application.Features.TypeOriginFeature.Commands.CreateTypeOriginCommand;
using SMART.ERP.Application.Features.TypeStatusFeature.Commands.CreateTypeStatusCommand;
using SMART.ERP.Application.Features.UnitOfMeasurementFeature.Commands.CreateUnitOfMeasurementCommand;
using SMART.ERP.Application.Features.UserFeature.Commands.CreateUserCommand;
using SMART.ERP.Application.Features.WarehouseFeature.Commands.CreateWarehouseCommand;
using SMART.ERP.Application.Features.WinReasonFeature.Commands.CreateWinReasonCommand;
using SMART.ERP.Application.Features.WishListFeature.Commands.CreateWishListCommand;
using SMART.ERP.Application.Features.WishListProductFeature.Commands.CreateWishListProductCommand;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region DTOs
            CreateMap<AdvisorGoal, AdvisorGoalDto>();
            CreateMap<AdvisorDepartment, AdvisorDepartmentDto>();
            CreateMap<Brand, BrandDto>();
            CreateMap<PriceList, PriceListDto>()
                .ForMember(d => d.ItemsCount, opt => opt.Ignore());
            CreateMap<Features.PriceListFeature.Commands.CreatePriceListCommand.CreatePriceListCommand, PriceList>()
                .ForMember(d => d.Id, opt => opt.Ignore())
                .ForMember(d => d.CreationDate, opt => opt.Ignore())
                .ForMember(d => d.CreatedBy, opt => opt.Ignore())
                .ForMember(d => d.ModificationDate, opt => opt.Ignore())
                .ForMember(d => d.ModificatedBy, opt => opt.Ignore())
                .ForMember(d => d.Items, opt => opt.Ignore());
            CreateMap<Category, CategoryDto>();
            CreateMap<DataSheet, DataSheetDto>();
            CreateMap<Status, StatusDto>();
            CreateMap<Product, ProductDto>();
            CreateMap<Provider, ProviderDto>();
            CreateMap<PurchaseBill, ProviderPurchaseBillLineDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : null));
            CreateMap<PurchaseOrder, ProviderPurchaseOrderLineDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : null));
            CreateMap<TypeEntity, TypeEntityDto>();
            CreateMap<TypeStatus, TypeStatusDto>();
            CreateMap<UnitOfMeasurement, UnitOfMeasurementDto>();
            CreateMap<Subcategory, SubcategoryDto>();
            CreateMap<ProductDataSheet, ProductDataSheetDto>();
            CreateMap<ProductImage, ProductImageDto>();
            CreateMap<City, CityDto>();
            CreateMap<Currency, CurrencyDto>();
            CreateMap<User, UserDto>();
            CreateMap<User, UserWalletDto>();
            CreateMap<User, OpportunityAdvisorDto>();
            CreateMap<User, SalesByAdvisorDto>();
            CreateMap<Role, RoleDto>();
            CreateMap<Gender, GenderDto>();
            CreateMap<Country, CountryDto>()
                .ForMember(x => x.Departments, opt => opt.MapFrom(x => x.Regions!.Count > 0 ? x.Regions.SelectMany(z => z.Departments!).ToList() : x.Departments));
            CreateMap<Department, DepartmentDto>();
            CreateMap<ProductFeature, ProductFeatureDto>();
            CreateMap<Company, CompanyDto>();
            CreateMap<Banner, BannerDto>();
            CreateMap<Region, RegionDto>();
            CreateMap<BranchOffices, BranchOfficeDto>();
            CreateMap<Opinion, OpinionDto>();
            CreateMap<MetaAdCampaign, MetaAdCampaignDto>();
            CreateMap<Heading, HeadingDto>();
            CreateMap<SocialReason, SocialReasonDto>();
            CreateMap<CustomerType, CustomerTypeDto>()
                .ForMember(d => d.PriceListName, opt => opt.MapFrom(s => s.PriceList != null ? s.PriceList.Name : null));
            CreateMap<Gender, GenderDto>();
            CreateMap<City, CityDto>();
            CreateMap<Department, ClientDepartmentDto>();
            CreateMap<Customer, CustomerDto>()
                .ForMember(d => d.PriceListName, opt => opt.MapFrom(s => s.PriceList != null ? s.PriceList.Name : null));
            CreateMap<Customer, BasicInfoCustomerDto>();
            CreateMap<Customer, ClientWalletDto>();
            CreateMap<Invoice, CustomerInvoiceLineDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : null));
            CreateMap<Quotation, CustomerQuotationLineDto>()
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status != null ? src.Status.Name : null));
            CreateMap<OpportunityStep, OpportunityStepDto>();
            CreateMap<Opportunity, OpportunityDto>();
            CreateMap<Opportunity, OpportunityWalletDto>();
            CreateMap<Opportunity, ActiveOpportunityDto>();
            CreateMap<DeliveryDirection, DeliveryDirectionDto>();
            CreateMap<FinancingPlan, FinancingPlanDto>();
            CreateMap<Opportunity, OpportunityDto>();
            CreateMap<QuoteProduct, QuoteProductDto>();
            CreateMap<QuoteProduct, UpdateOpportunityQuoteDto>();
            CreateMap<WishList, WishListDto>();
            CreateMap<WishListProduct, WishListProductDto>();
            CreateMap<WishListProduct, CreateWishListProductDto>();
            CreateMap<HeroSlider, HeroSliderDto>();
            CreateMap<Category, CategoryWithHeroSliderDto>();
            CreateMap<Product, ResumeProductDto>();
            CreateMap<ProductImage, ResumeProductImageDto>();
            CreateMap<ProductDataSheet, ResumeProductDataSheetDto>();
            CreateMap<Prospect, ProspectDto>();
            CreateMap<ProspectQuoteProduct, ProspectQuoteProductDto>();
            CreateMap<ProspectStep, ProspectStepDto>();
            CreateMap<SaleOrder, SaleOrderDto>();
            CreateMap<SaleOrderProduct, SaleOrderProductDto>();
            CreateMap<Product, BasicDetailProductDto>();
            CreateMap<TypeOrigin, TypeOriginDto>();
            CreateMap<InterestLevel, InterestLevelDto>();
            CreateMap<DocumentType, DocumentTypeDto>();
            CreateMap<TypeActivity, TypeActivityDto>();
            CreateMap<OpportunityActivity, OpportunityActivityDto>();
            CreateMap<OpportunityDocument, OpportunityDocumentDto>();
            CreateMap<OpportunityComment, OpportunityCommentDto>();
            CreateMap<WinReason, WinReasonDto>();
            CreateMap<LossReason, LossReasonDto>();
            CreateMap<Notification, NotificationDto>();
            CreateMap<TypeStatus, ResumeTypeStatusDto>();
            CreateMap<Cai, CaiDto>();
            CreateMap<Prefix, PrefixDto>();
            CreateMap<Tax, TaxDto>();
            CreateMap<Quotation, QuotationDto>();
            CreateMap<ProductOffered, ProductOfferedDto>().ReverseMap();
            CreateMap<ProductToOfferdDto, ProductOffered>();
            CreateMap<QuotationSnapshot, QuotationSnapshotDto>();

            // Quotation Preview mappings
            CreateMap<QuotationComment, QuotationCommentDto>()
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : null));
            CreateMap<QuotationItemObservation, QuotationItemObservationDto>();
            CreateMap<ProductOffered, ProductOfferedPreviewDto>()
                .ForMember(dest => dest.TaxRate, opt => opt.MapFrom(src => src.Tax != null ? src.Tax.Rate : 0));
            CreateMap<QuotationSnapshot, QuotationSnapshotDetailDto>()
                .ForMember(dest => dest.SnapshotData, opt => opt.Ignore());
            CreateMap<Quotation, QuotationSnapshotDataDto>();
            CreateMap<ProductOffered, ProductOfferedSnapshotDto>();
            CreateMap<InternalDocument, InternalDocumentDto>();
            CreateMap<Warehouse, WarehouseDto>();
            CreateMap<InventoryDistribution, InventoryDistributionDto>();
            CreateMap<InventoryDistributionDto, InventoryDistribution>();
            CreateMap<WarehouseDto, Warehouse>();
            CreateMap<InventoryInputType, InventoryInputTypeDto>();
            CreateMap<InventoryInput, InventoryInputDto>();
            CreateMap<InventoryInputDto, InventoryInput>();
            CreateMap<ProductEntry, ProductEntryDto>().ReverseMap();
            CreateMap<CreateProductEntryDto, ProductEntryDto>();
            CreateMap<ProductEntryDto, CreateProductEntryDto>();
            CreateMap<CreateProductEntryDto, ProductEntry>();
            CreateMap<Invoice, InvoiceDto>();
            CreateMap<InvoiceDto, Invoice>();
            CreateMap<ProductSold, ProductSoldDto>().ReverseMap();
            CreateMap<ProductToSellDto, ProductSold>();
            CreateMap<ProductOffered, ProductSold>();
            CreateMap<Bank, BankDto>();
            CreateMap<InternalBankAccount, InternalBankAccountDto>();
            CreateMap<BillPayment, BillPaymentDto>();
            CreateMap<TypeOfPaymentMethod, TypeOfPaymentMethodDto>();
            CreateMap<TypeProvider, TypeProviderDto>();
            CreateMap<PurchaseOrder, PurchaseOrderDto>();
            CreateMap<ProductToPurchase, ProductToPurchaseDto>().ReverseMap();
            CreateMap<ProductToBuyDto, ProductToPurchase>();
            CreateMap<PurchaseBillDto, PurchaseBill>().ReverseMap();
            CreateMap<PurchaseBillPayment, PurchaseBillPaymentDto>().ReverseMap();
            CreateMap<MajorExpenseAccount, MajorExpenseAccountDto>().ReverseMap();
            CreateMap<MajorIncomeAccount, MajorIncomeAccountDto>().ReverseMap();
            CreateMap<IncomeAccount, IncomeAccountDto>().ReverseMap();
            CreateMap<ExpenseAccount, ExpenseAccountDto>().ReverseMap();
            CreateMap<ProductPurchasePriceLogDto, ProductPurchasePriceLog>().ReverseMap();
            CreateMap<Project, ProjectDto>().ReverseMap();
            CreateMap<NonBillableExpense, NonBillableExpenseDto>().ReverseMap();
            CreateMap<NonBillableExpensePayment, NonBillableExpensePaymentDto>().ReverseMap();
            CreateMap<ProductToBuyDto, ProductEntry>();
            CreateMap<DailyClose, DailyCloseDto>().ReverseMap();
            CreateMap<ResumePayment, ResumePaymentDto>().ReverseMap();
            CreateMap<MonthlyPurchaseDeclaration, MonthlyPurchaseDeclarationDto>().ReverseMap();
            CreateMap<DeclaratedPurchaseBill, DeclaratedPurchaseBillDto>().ReverseMap();
            CreateMap<MonthlySaleDeclaration, MonthlySaleDeclarationDto>().ReverseMap();
            CreateMap<DeclaredSaleInvoice, DeclaredSaleInvoiceDto>().ReverseMap();
            CreateMap<InvoicePaymentType, InvoicePaymentTypeDto>().ReverseMap();
            CreateMap<Discount,DiscountDto>().ReverseMap();
            CreateMap<EcommerceUser, EcommerceUserDto>().ReverseMap();
            CreateMap<LogEcommerceUser, LogEcommerceUserDto>()
                .ForMember(dest => dest.ActionTypeName, opt => opt.MapFrom(src =>
                    src.ActionType == 1 ? "Inicio de sesión" :
                    src.ActionType == 2 ? "Cambio de contraseña" :
                    src.ActionType == 3 ? "Actualización de perfil" :
                    src.ActionType == 4 ? "Actualización de foto de perfil" : "Desconocido"));
            CreateMap<AssociatedCompany, AssociatedCompanyDto>();
            CreateMap<PaymentMethod, PaymentMethodDto>();
            CreateMap<Cart, CartDto>().ReverseMap();
            CreateMap<CartItem, CartItemDto>().ReverseMap();
            CreateMap<ProjectAttachmentCategory, ProjectAttachmentCategoryDto>();
            CreateMap<ProjectAttachment, ProjectAttachmentDto>();
            CreateMap<ChatSession, ChatSessionDto>()
                .ForMember(dest => dest.AssignedAdminName, opt => opt.MapFrom(src => src.AssignedAdminUser != null ? src.AssignedAdminUser.FullName : null));
            CreateMap<ChatMessage, ChatMessageDto>();
            #endregion

            #region Commands
            CreateMap<CreateBrandCommand, Brand>();
            CreateMap<CreateCategoryCommand, Category>();
            CreateMap<CreateSubcategoryCommand, Subcategory>();
            CreateMap<CreateCustomerCommand, Customer>();
            CreateMap<CreateProviderCommand, Provider>();
            CreateMap<CreateDataSheetCommand, DataSheet>();
            CreateMap<CreateBaseProductCommand, Product>();
            CreateMap<CreateProductFtrCommand, ProductFeature>();
            CreateMap<CreateProductDataSheetCommand, ProductDataSheet>();
            CreateMap<CreateProductImageCommand, ProductImage>();
            CreateMap<CreateUnitOfMeasurementCommand, UnitOfMeasurement>();
            CreateMap<CreateUserCommand, User>();
            CreateMap<CreateMessageCommand, Message>();
            CreateMap<CreateCompanyCommand, Company>();
            CreateMap<CreateBannerCommand, Banner>();
            CreateMap<CreateOpinionCommand, Opinion>();
            CreateMap<CreateBranchOfficeCommand, BranchOffices>();
            CreateMap<CreateProductDataSheetDto, ProductDataSheet>();
            CreateMap<CreateQuoteProductDto, QuoteProduct>();
            CreateMap<CreateProductFeatureDto, ProductFeature>();
            CreateMap<CreateProductImageDto, ProductImage>();
            CreateMap<CreateFinancingPlanCommand, FinancingPlan>();
            CreateMap<CreateOpportunityCommand, Opportunity>();
            CreateMap<CreateRegionCommand, Region>();
            CreateMap<CreateQuoteProductCommand, QuoteProduct>();
            CreateMap<CreateWishListCommand, WishList>();
            CreateMap<CreateWishListProductDto, WishListProduct>();
            CreateMap<CreateWishListProductCommand, WishListProduct>();
            CreateMap<CreateHeroSliderCommand, HeroSlider>();
            CreateMap<UpdateOpportunityQuoteDto, QuoteProduct>();
            CreateMap<CreateDeliveryDirectionCommand, DeliveryDirection>();
            CreateMap<CreateTypeOriginCommand, TypeOrigin>();
            CreateMap<CreateTypeStatusCommand, TypeStatus>();
            CreateMap<CreateInterestLevelCommand, InterestLevel>();
            CreateMap<CreateDocumentTypeCommand, DocumentType>();
            CreateMap<CreateTypeActivityCommand, TypeActivity>();
            CreateMap<CreateOpportunityActivityCommand, OpportunityActivity>();
            CreateMap<CreateOpportunityCommentCommand, OpportunityComment>();
            CreateMap<CreateOpportunityDocumentCommand, OpportunityDocument>();
            CreateMap<CreateWinReasonCommand, WinReason>();
            CreateMap<CreateStatusCommand, Status>();
            CreateMap<CreateLossReasonCommand, LossReason>();
            CreateMap<CreateCountryCommand, Country>();
            CreateMap<CreateCityCommand, City>();
            CreateMap<CreateDepartmentCommand, Department>();
            CreateMap<CreateCaiCommand, Cai>();
            CreateMap<CreatePrefixCommand, Prefix>();
            CreateMap<CreateTaxCommand, Tax>();
            CreateMap<CreateQuotationCommand, Quotation>();
            CreateMap<CreateInventoryInputTypeCommand, InventoryInputType>();
            CreateMap<CreateInventoryInputCommand, InventoryInput>();
            CreateMap<CreateWarehouseCommand, Warehouse>();
            CreateMap<CreateInvoiceCommand, Invoice>();
            CreateMap<CreateBankCommand, Bank>();
            CreateMap<CreateInternalBankAccountCommand, InternalBankAccount>();
            CreateMap<CreateBillPaymentCommand, BillPayment>();
            CreateMap<CopyQuotationFromIdCommand, Quotation>();
            CreateMap<CreatePurchaseOrderCommand, PurchaseOrder>();
            CreateMap<CreatePurchaseBillFromPurchaseOrderDetailPageCommand, PurchaseBill>();
            CreateMap<CreatePurchaseBillCommand, PurchaseBill>();
            CreateMap<CreatePurchaseBillPaymentCommand, PurchaseBillPayment>();
            CreateMap<CreateMajorExpenseAccountCommand, MajorExpenseAccount>();
            CreateMap<CreateMajorIncomeAccountCommand, MajorIncomeAccount>();
            CreateMap<CreateIncomeAccountCommand, IncomeAccount>();
            CreateMap<CreateExpenseAccountCommand, ExpenseAccount>();
            CreateMap<CreateProjectAttachmentCategoryCommand, ProjectAttachmentCategory>();
            CreateMap<CreateProjectAttachmentCommand, ProjectAttachment>();
            CreateMap<CreateProjectCommand, Project>();
            CreateMap<CreateNonBillableExpenseCommand, NonBillableExpense>();
            CreateMap<CreateNonBillableExpensePaymentCommand, NonBillableExpensePayment>();
            CreateMap<CancelInvoiceCommand, InvoiceDto>();
            CreateMap<CreateInventoryInputByPurchaseOrderIdCommand, InventoryInput>();
            CreateMap<CreateDailyCloseCommand, DailyClose>();
            CreateMap<CreateInvoiceFromPosScreenCommand, Invoice>();
            CreateMap<CreateDiscountCommand, Discount>();
            CreateMap<CreateEcommerceUserCommand, EcommerceUser>();
            CreateMap<AdminCreateEcommerceUserCommand, EcommerceUser>();
            CreateMap<CreateAssociatedCompanyCommand, AssociatedCompany>();
            CreateMap<CreatePaymentMethodCommand, PaymentMethod>()
                .ForMember(dest => dest.EncryptedCardNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Last4Digits, opt => opt.Ignore());

            // Dropshipping mappings
            CreateMap<ShippingCostConfiguration, ShippingCostDto>()
                .ForMember(dest => dest.SourceWarehouseName, opt => opt.MapFrom(src => src.SourceWarehouse != null ? src.SourceWarehouse.Name : null))
                .ForMember(dest => dest.SourceProviderName, opt => opt.MapFrom(src => src.SourceProvider != null ? src.SourceProvider.Name : null))
                .ForMember(dest => dest.SourceCityName, opt => opt.MapFrom(src => src.SourceCity != null ? src.SourceCity.Name : null))
                .ForMember(dest => dest.DestinationCityName, opt => opt.MapFrom(src => src.DestinationCity != null ? src.DestinationCity.Name : null))
                .ForMember(dest => dest.DestinationDepartmentName, opt => opt.MapFrom(src => src.DestinationDepartment != null ? src.DestinationDepartment.Name : null))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : null));

            CreateMap<ProviderWarehouse, ProviderWarehouseDto>()
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.Provider != null ? src.Provider.Name : string.Empty))
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty));

            #endregion

            #region RecurringInvoiceTemplate
            CreateMap<RecurringInvoiceTemplate, RecurringInvoiceTemplateDto>().ReverseMap();
            CreateMap<RecurringInvoiceTemplateItem, RecurringInvoiceTemplateItemDto>().ReverseMap();
            CreateMap<RecurringInvoiceLog, RecurringInvoiceLogDto>().ReverseMap();
            #endregion
        }
    }
}
