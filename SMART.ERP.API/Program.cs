using Asp.Versioning;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using SMART.ERP.API.Extensions;
using SMART.ERP.Application;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Services.QuotationPdfService;
using SMART.ERP.Infrastructure;
using SMART.ERP.Infrastructure.Repository;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            // Configurar orígenes permitidos usando SetIsOriginAllowed para combinar todos los casos
            policy.SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrEmpty(origin))
                    return false;
                    
                try
                {
                    var uri = new Uri(origin);
                    var host = uri.Host.ToLower();
                    
                    // Dominios específicos de smartbusiness.site
                    if (host == "www.smartbusiness.site" ||
                        host == "admin.smartbusiness.site" ||
                        host == "api.smartbusiness.site")
                    {
                        return true;
                    }
                    
                    // Localhost para desarrollo (con cualquier puerto)
                    if (host == "localhost" || host == "127.0.0.1")
                    {
                        return true;
                    }
                    
                    // Subdominios de vercel.app
                    if (host.EndsWith(".vercel.app"))
                    {
                        return true;
                    }
                    
                    return false;
                }
                catch
                {
                    return false;
                }
            })
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddApiVersioning(
                    options =>
                    {
                        options.ReportApiVersions = true;
                        options.Policies.Sunset(0.9)
                                        .Effective(DateTimeOffset.Now.AddDays(60))
                                        .Link("policy.html")
                                            .Title("Versioning Policy")
                                            .Type("text/html");
                    })
                .AddMvc()
                .AddApiExplorer(
                    options =>
                    {
                        options.GroupNameFormat = "'v'VVV";
                        options.SubstituteApiVersionInUrl = true;
                    });
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.OperationFilter<SwaggerDefaultValues>();
        var fileName = typeof(Program).Assembly.GetName().Name + ".xml";
        var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
        options.IncludeXmlComments(filePath);
    });

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

// Configurar Kestrel solo si no estamos en producción
// En producción, ASPNETCORE_URLS maneja la configuración del puerto
// No configurar Kestrel manualmente en producción para evitar conflictos de "address already in use"
// IMPORTANTE: En producción, NO cargar configuración de Kestrel desde appsettings para evitar que sobrescriba ASPNETCORE_URLS
if (!builder.Environment.IsProduction())
{
    builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Kestrel"));
}

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure();
        });
});


builder.Services.AddTransient(typeof(IRepositoryAsync<>), typeof(CustomRepositoryAsync<>));
builder.Services.AddTransient(typeof(IReadRepositoryAsync<>), typeof(CustomRepositoryAsync<>));
builder.Services.AddTransient<IQuotationPdfService, SMART.ERP.Infrastructure.Services.QuotationPdfService.QuotationPdfService>();

// Inventory module (Ventix-style)
builder.Services.AddScoped<SMART.ERP.Application.Repository.IUnitOfWork, SMART.ERP.Infrastructure.Repository.UnitOfWork>();
builder.Services.AddScoped<SMART.ERP.Application.Services.InventoryMovementService.IInventoryMovementService, SMART.ERP.Infrastructure.Services.InventoryMovementService.InventoryMovementService>();
builder.Services.AddScoped<SMART.ERP.Application.Services.ProductCompositionService.IProductCompositionService, SMART.ERP.Infrastructure.Services.ProductCompositionService.ProductCompositionService>();
builder.Services.AddScoped<SMART.ERP.Application.Services.ProductCacheInvalidator.IProductCacheInvalidator, SMART.ERP.Application.Services.ProductCacheInvalidator.ProductCacheInvalidator>();

// Módulo contable: contabilización automática (Fase 2).
builder.Services.AddScoped<SMART.ERP.Application.Services.AccountingPostingService.IAccountingPostingService, SMART.ERP.Infrastructure.Services.AccountingPostingService.AccountingPostingService>();
builder.Services.AddTransient<SMART.ERP.Application.Services.KardexReportService.IKardexPdfService, SMART.ERP.Infrastructure.Services.KardexReportService.KardexPdfService>();
builder.Services.AddTransient<SMART.ERP.Application.Services.KardexReportService.IKardexExcelService, SMART.ERP.Infrastructure.Services.KardexReportService.KardexExcelService>();
builder.Services.AddTransient<SMART.ERP.Application.Services.VirtualStock.IVirtualStockExcelReader, SMART.ERP.Infrastructure.Services.VirtualStock.VirtualStockExcelReader>();
builder.Services.AddTransient<SMART.ERP.Application.Services.ExcelImportService.IExcelImportService, SMART.ERP.Infrastructure.Services.ExcelImportService.ExcelImportService>();

// Brochures comerciales (catálogo PDF para redes sociales).
// El servicio de imágenes es Singleton: mantiene una caché acotada de fotos ya
// reescaladas, que se amortiza cuando el usuario regenera variando un filtro.
builder.Services.AddSingleton<SMART.ERP.Application.Services.BrochureImageService.IBrochureImageService, SMART.ERP.Application.Services.BrochureImageService.BrochureImageService>();
builder.Services.AddTransient<SMART.ERP.Application.Services.BrochureDataService.IBrochureDataService, SMART.ERP.Application.Services.BrochureDataService.BrochureDataService>();
builder.Services.AddTransient<SMART.ERP.Application.Services.BrochurePdfService.IBrochurePdfService, SMART.ERP.Infrastructure.Services.BrochurePdfService.BrochurePdfService>();

builder.WebHost.UseSentry(opts =>
{
    opts.SetBeforeSend((@event, hint) =>
    {
        @event.ServerName = null;
        return @event;
    });
});
builder.Services.AddStackExchangeRedisOutputCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisServer");
    options.InstanceName = "SB_cache_";
});
builder.Services.AddOutputCache(opt =>
    {
        //PANEL ADMIN CACHE
        opt.AddPolicy("cache_brands", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_brands").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_categories", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_categories").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_cities", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_cities").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_departments", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_departments").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_subCategories", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_subCategories").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_branchOffices", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_branchOffices").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_cais", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_cais").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_products", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_products").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_pricelists", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_pricelists").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_banks", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_banks").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_customer", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_customer").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_genders", builder => builder.Expire(TimeSpan.FromDays(10)));
        opt.AddPolicy("cache_headings", builder => builder.Expire(TimeSpan.FromDays(10)));
        opt.AddPolicy("cache_internalBankAccounts", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_internalBankAccounts").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_socialReasons", builder => builder.Expire(TimeSpan.FromDays(10)));
        opt.AddPolicy("cache_providers", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_providers").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_taxes", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_taxes").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_typeOfPayment", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_typeOfPayment").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_statuses", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_statuses").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_major_expense_account", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_major_expense_account").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_major_income_account", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_major_income_account").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_income_account", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_income_account").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_expense_account", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_expense_account").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_nonBillableExpense", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_nonBillableExpense").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_creditCardPayment", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_creditCardPayment").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_project", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_project").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_dailyClose", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_dailyClose").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_monthlyPurchaseDeclaration", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_monthlyPurchaseDeclaration").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_monthlySaleDeclaration", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_monthlySaleDeclaration").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        //MÓDULO CONTABLE
        opt.AddPolicy("cache_ledger_accounts", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_ledger_accounts").SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_fiscal_periods", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_fiscal_periods"));
        opt.AddPolicy("cache_accounting_reports", builder => builder.Expire(TimeSpan.FromMinutes(30)).Tag("cache_accounting_reports").SetVaryByQuery(["*"]));
        //ECOMMERCE CACHE
        opt.AddPolicy("cache_getAllNavCategories", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_getAllNavCategories"));
        opt.AddPolicy("cache_productsEcommerce", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_productsEcommerce"));
        opt.AddPolicy("cache_productsBySameCategorySlug", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_productsBySameCategorySlug"));
        opt.AddPolicy("cache_productsByCategorySlug", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_productsByCategorySlug"));
        opt.AddPolicy("cache_productsBySubCategorySlug", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_productsBySubCategorySlug"));
        
        // Políticas de cache optimizadas para búsqueda
        opt.AddPolicy("cache_productSearch", builder => builder.Expire(TimeSpan.FromMinutes(30)).Tag("cache_productSearch").SetVaryByQuery(["*"]));
        opt.AddPolicy("cache_searchSuggestions", builder => builder.Expire(TimeSpan.FromHours(2)).Tag("cache_searchSuggestions").SetVaryByQuery(["searchTerm", "limit"]));
    });

// Configurar ForwardedHeaders para proxy reverso (Dokploy/Nginx)
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
    // Permitir todos los proxies conocidos (para Dokploy)
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
    // No requerir simetría en headers
    options.RequireHeaderSymmetry = false;
});

var app = builder.Build();
app.UseSentryTracing();

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders();
}
app.UseErrorHandlingMiddleware();
// Comentar HttpsRedirection si Dokploy/Nginx ya maneja HTTPS
// if (!app.Environment.IsDevelopment())
// {
//     app.UseHttpsRedirection();
// }
app.UseStaticFiles();
app.UseRouting();

// Configurar CORS ANTES de WebSockets y otros middlewares
app.UseCors();

// Configurar WebSockets con los mismos orígenes permitidos
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
webSocketOptions.AllowedOrigins.Add("http://localhost:4200");
webSocketOptions.AllowedOrigins.Add("https://localhost:3000");
webSocketOptions.AllowedOrigins.Add("http://localhost:3000");
webSocketOptions.AllowedOrigins.Add("https://admin.smartbusiness.site");
webSocketOptions.AllowedOrigins.Add("https://www.smartbusiness.site");
webSocketOptions.AllowedOrigins.Add("https://api.smartbusiness.site");
app.UseWebSockets(webSocketOptions);

app.UseOutputCache();
app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            var descriptions = app.DescribeApiVersions();
            foreach (var description in descriptions)
            {
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
            }
        });
}

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("hub/notification");
    endpoints.MapHub<ChatHub>("hub/chat");
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<DatabaseContext>();

    logger.LogInformation("Applying pending EF Core migrations...");
    await context.Database.MigrateAsync();
    logger.LogInformation("Migrations applied successfully.");

    try
    {
        await SMART.ERP.Infrastructure.Seeders.DiscountSeed.SeedAsync(context);
        await SMART.ERP.Infrastructure.Seeders.DropshippingSeed.SeedAsync(context);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while seeding data");
    }
}

app.Run();
