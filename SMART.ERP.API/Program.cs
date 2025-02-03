using Asp.Versioning;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using SMART.ERP.API.Extensions;
using SMART.ERP.API.Middlewares;
using SMART.ERP.Application;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.SignalRHub;
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
            policy.WithOrigins("https://*.vercel.app")
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
            policy.WithOrigins("https://www.smartbusiness.site", "https://admin.smartbusiness.site")
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
            policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
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

builder.Services.AddSignalR();

builder.Services.Configure<KestrelServerOptions>(builder.Configuration.GetSection("Kestrel"));

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure();
        });
});


builder.Services.AddTransient(typeof(IRepositoryAsync<>), typeof(CustomRepositoryAsync<>));

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
        opt.AddPolicy("cache_brands", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_categories", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_cities", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_departments", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_subCategories", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_branchOffices", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_cais", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_products", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_banks", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_customer", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_genders", builder => builder.Expire(TimeSpan.FromDays(10)));
        opt.AddPolicy("cache_headings", builder => builder.Expire(TimeSpan.FromDays(10)));
        opt.AddPolicy("cache_internalBankAccounts", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_socialReasons", builder => builder.Expire(TimeSpan.FromDays(10)));
        opt.AddPolicy("cache_providers", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_taxes", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_typeOfPayment", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_statuses", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_major_expense_account", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_major_income_account", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_income_account", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_expense_account", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_nonBillableExpense", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_dailyClose", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        opt.AddPolicy("cache_monthlyPurchaseDeclaration", builder => builder.Expire(TimeSpan.FromDays(10)).SetVaryByQuery(["PageNumber", "PageSize", "Parameter", "Order", "Column", "All"]));
        //ECOMMERCE CACHE
        opt.AddPolicy("cache_getAllNavCategories", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_getAllNavCategories"));
        opt.AddPolicy("cache_productsEcommerce", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_productsEcommerce"));
        opt.AddPolicy("cache_productsBySameCategorySlug", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_productsBySameCategorySlug"));
        opt.AddPolicy("cache_productsByCategorySlug", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_productsByCategorySlug"));
        opt.AddPolicy("cache_productsBySubCategorySlug", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_productsBySubCategorySlug"));
    });
var app = builder.Build();
app.UseSentryTracing();

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
}
app.UseErrorHandlingMiddleware();
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseStaticFiles(); app.UseStaticFiles();
app.UseRouting();
var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
webSocketOptions.AllowedOrigins.Add("http://localhost:4200");
webSocketOptions.AllowedOrigins.Add("https://admin.smartbusiness.site");
webSocketOptions.AllowedOrigins.Add("https://www.smartbusiness.site");
app.UseWebSockets(webSocketOptions);
app.UseCors();

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
});
app.Run();
