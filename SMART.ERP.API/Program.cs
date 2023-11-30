using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SMART.ERP.API.Extensions;
using SMART.ERP.Application;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.SignalRHub;
using Quartz;
using SMART.ERP.Infrastructure;
using SMART.ERP.Infrastructure.Repository;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            //policy.WithOrigins("https://*.vercel.app").SetIsOriginAllowedToAllowWildcardSubdomains().AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            policy.WithOrigins("https://www.smartbusiness.site", "https://admin.smartbusiness.site").SetIsOriginAllowedToAllowWildcardSubdomains().AllowAnyHeader().AllowAnyMethod().AllowCredentials();
            policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
            .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
        });
});
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationLayer(builder.Configuration);
builder.Services.AddApiVersioningExtension();
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddSignalR();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "SMART ERP",
        Description = "Una API web de ASP.NET Core para gestionar el ERP del lado administrativo ",
        Contact = new OpenApiContact
        {
            Name = "Contacto",
            Url = new Uri("https://github.com/Smart-Business-HN")
        },
    });
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "SMART ERP",
        Description = "Una API web de ASP.NET Core para gestionar el E-Commerce",
        Contact = new OpenApiContact
        {
            Name = "Contacto",
            Url = new Uri("https://github.com/Smart-Business-HN")
        },
    });
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

});

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
    opts.BeforeSend = @event =>
    {
        @event.ServerName = null;
        return @event;
    };
});
builder.Services.AddStackExchangeRedisOutputCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisServer");
    options.InstanceName = "SB_cache_";
});
builder.Services.AddOutputCache(opt =>
    {
        //PANEL ADMIN CACHE
        opt.AddPolicy("cache_brands", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_brands"));
        opt.AddPolicy("cache_categories", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_categories"));
        opt.AddPolicy("cache_cities", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_cities"));
        opt.AddPolicy("cache_departments", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_departments"));
        opt.AddPolicy("cache_subCategories", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_subCategories"));
        opt.AddPolicy("cache_salesAdvisors", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_salesAdvisors"));
        opt.AddPolicy("cache_branchOffices", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_branchOffices"));
        opt.AddPolicy("cache_cais", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_cais"));
        opt.AddPolicy("cache_products", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_products"));
        opt.AddPolicy("cache_banks", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_banks"));
        opt.AddPolicy("cache_customer", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_customer"));
        opt.AddPolicy("cache_genders", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_genders"));
        opt.AddPolicy("cache_headings", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_headings"));
        opt.AddPolicy("cache_internalBankAccounts", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_internalBankAccounts"));
        opt.AddPolicy("cache_socialReasons", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_socialReasons"));
        opt.AddPolicy("cache_providers", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_providers"));
        opt.AddPolicy("cache_taxes", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_tax"));
        opt.AddPolicy("cache_typeOfPayment", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_typeOfPayment"));
        opt.AddPolicy("cache_statuses", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_statuses"));
        //ECOMMERCE CACHE
        opt.AddPolicy("cache_getAllNavCategories", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_getAllNavCategories"));
        opt.AddPolicy("cache_productsEcommerce", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_productsEcommerce"));
        opt.AddPolicy("cache_productsBySameCategorySlug", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_productsBySameCategorySlug"));
        opt.AddPolicy("cache_producsByCategorySlug", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_producsByCategorySlug"));
        opt.AddPolicy("cache_productsBySubCategorySlug", builder => builder.Expire(TimeSpan.FromDays(10)).Tag("cache_productsBySubCategorySlug"));
    }); 
var app = builder.Build();
app.UseSentryTracing();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

//webSocketOptions.AllowedOrigins.Add("http://localhost:4200");
webSocketOptions.AllowedOrigins.Add("https://admin.smartbusiness.site");
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseOutputCache();
app.UseErrorHandlingMiddleware();
app.UseWebSockets(webSocketOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });
}
app.UseStaticFiles();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.MapHub<NotificationHub>("hub/notification"); 
app.MapControllers();
app.Run();
