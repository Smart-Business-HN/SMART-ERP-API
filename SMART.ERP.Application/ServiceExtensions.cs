using Azure.Storage.Blobs;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using SMART.ERP.Application.Behaviours;
using SMART.ERP.Application.Jobs.AdvisorGoalJob;
using SMART.ERP.Application.Jobs.LogSessionJob;
using SMART.ERP.Application.Jobs.LongLivedOpportunitiesJob;
using SMART.ERP.Application.Jobs.RecurringInvoiceJob;
using SMART.ERP.Application.Services.InvoiceGenerationService;
using SMART.ERP.Application.Services;
using SMART.ERP.Application.Services.AssignUserToOpportunityService;
using SMART.ERP.Application.Services.AssignUserToProspectService;
using SMART.ERP.Application.Services.BlobStorageService;
using SMART.ERP.Application.Services.GoogleCalendarService;
using SMART.ERP.Application.Services.HeaderService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Services.MetaPostService;
using SMART.ERP.Application.Services.CardEncryptionService;
using SMART.ERP.Application.Services.NewEncryptionService;
using SMART.ERP.Application.Services.RegisterClientService;
using SMART.ERP.Application.Services.ShippingCostCalculator;
using SMART.ERP.Application.Services.WarehouseSelection;
using SMART.ERP.Application.Services.VirtualStock;
using SMART.ERP.Application.Services.QuotationDiffService;
using SMART.ERP.Application.Wrappers;
using SMART.ERP.Domain.Settings;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace SMART.ERP.Application
{
    public static class ServiceExtensions
    {
        public static void AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
            //---------- Configure Quartz Settings ------
            services.AddQuartz(x =>
            {
                //x.UseMicrosoftDependencyInjectionJobFactory();
                x.AddJob<AdvisorGoalJob>(opts => opts.WithIdentity(AdvisorGoalJob.Key));
                x.AddJob<LongLivedOpportunitiesJob>(opts => opts.WithIdentity(LongLivedOpportunitiesJob.Key));
                x.AddJob<LogSessionJob>(opts => opts.WithIdentity(LogSessionJob.LogJobKey));
                x.AddJob<RecurringInvoiceJob>(opts => opts.WithIdentity(RecurringInvoiceJob.Key));

                x.AddTrigger(opts => opts
                .ForJob(AdvisorGoalJob.Key)
                .WithIdentity("advisor-goal-job-trigger")
                .WithCronSchedule("59 59 23 3 * ?"));

                x.AddTrigger(opts => opts
                .ForJob(LongLivedOpportunitiesJob.Key)
                .WithIdentity("long-lived-opportunities-job-trigger")
                .WithCronSchedule("0 1 0 1 1/1 ? *"));

                x.AddTrigger(opts => opts
                .ForJob(LogSessionJob.LogJobKey)
                .WithIdentity("log-session-job-trigger")
                .WithCronSchedule("0 59 16 1/1 * ? *"));

                x.AddTrigger(opts => opts
                .ForJob(RecurringInvoiceJob.Key)
                .WithIdentity("recurring-invoice-job-trigger")
                .WithCronSchedule("0 0 6 * * ?"));

            });
            services.AddQuartzHostedService(x => x.WaitForJobsToComplete = true);
            // ---------------- End -----------------
            services.AddHttpContextAccessor();
            services.AddAutoMapper(cfg =>
            {
                // AutoMapper 15+ es de licencia comercial. Configurar la clave en
                // appsettings ("AutoMapper:LicenseKey") cuando esté disponible.
                var autoMapperLicense = configuration["AutoMapper:LicenseKey"];
                if (!string.IsNullOrWhiteSpace(autoMapperLicense))
                {
                    cfg.LicenseKey = autoMapperLicense;
                }
            }, Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(cf =>
            {
                cf.RegisterServicesFromAssembly(typeof(ServiceExtensions).Assembly);
                // MediatR 13+ es de licencia comercial. Configurar la clave en
                // appsettings ("MediatR:LicenseKey") cuando esté disponible.
                var mediatRLicense = configuration["MediatR:LicenseKey"];
                if (!string.IsNullOrWhiteSpace(mediatRLicense))
                {
                    cf.LicenseKey = mediatRLicense;
                }
            });
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.Configure<MetaSettings>(configuration.GetSection("MetaSettings"));
            services.Configure<MetaAdSettings>(configuration.GetSection("MetaAdSettings"));
            services.Configure<GoogleCalendarSettings>(configuration.GetSection("GoogleCalendar"));
            services.Configure<EncryptionSettings>(configuration.GetSection("EncryptionSettings"));
            // configure blob storage service
            services.AddScoped(_ =>
            {
                return new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage"));
            });
            services.AddTransient<INewEncryptionService, NewEncryptionService>();
            services.AddTransient<ICardEncryptionService, CardEncryptionService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IBlobStorageService, BlobStorageService>();
            services.AddTransient<IRegisterClientService, RegisterClientService>();
            services.AddTransient<IHeaderService, HeaderService>();
            services.AddTransient<IJwtService, JwtService>();
            services.AddTransient<IAssignUserToOpportunityService, AssignUserToOpportunityService>();
            services.AddTransient<IAssignUserToProspectService, AssignUserToProspectService>();
            services.AddTransient<IMetaPostService, MetaPostService>();
            services.AddTransient<IGoogleCalendarService, GoogleCalendarService>();
            services.AddTransient<IProductPricingService, ProductPricingService>();
            services.AddTransient<IShippingCostCalculatorService, ShippingCostCalculatorService>();
            services.AddTransient<IWarehouseSelectionService, WarehouseSelectionService>();
            services.AddTransient<IVirtualStockService, VirtualStockService>();
            services.AddTransient<IInvoiceGenerationService, InvoiceGenerationService>();
            services.AddTransient<IQuotationDiffService, Services.QuotationDiffService.QuotationDiffService>();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]!)),
                };

                o.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();
                        c.Response.StatusCode = 401;
                        c.Response.ContentType = "application/json";
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {

                        context.HandleResponse();
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                        }
                        var result = JsonSerializer.Serialize(new Response<string>("Usted no esta autorizado para ejecutar esta acción"));
                        return context.Response.WriteAsync(result);
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        var result = JsonSerializer.Serialize(new Response<string>("Usted no tiene permisos para ejecutar esta acción"));
                        return context.Response.WriteAsync(result);
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/hub/notification") || path.StartsWithSegments("/hub/chat")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
