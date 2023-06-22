using Azure.Storage.Blobs;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SMART.ERP.Application.DTOs.Rootcloud;
using SMART.ERP.Application.Jobs.AdvisorGoalJob;
using SMART.ERP.Application.Jobs.LongLivedOpportunitiesJob;
using SMART.ERP.Application.Jobs.RootcloudJob;
using SMART.ERP.Application.Services.AssignUserToOpportunityService;
using SMART.ERP.Application.Services.AssignUserToProspectService;
using SMART.ERP.Application.Services.BlobStorageService;
using SMART.ERP.Application.Services.GoogleCalendarService;
using SMART.ERP.Application.Services.HeaderService;
using SMART.ERP.Application.Services.JwtService;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Services.MetaPostService;
using SMART.ERP.Application.Services.NewEncryptionService;
using SMART.ERP.Application.Wrappers;
using Quartz;
using SMART.ERP.Application.Behaviours;
using SMART.ERP.Application.Jobs.LogSessionJob;
using SMART.ERP.Application.Jobs.RootcloudJob;
using SMART.ERP.Application.Services.AssignUserToOpportunityService;
using SMART.ERP.Application.Services.RegisterClientService;
using SMART.ERP.Application.Services.Rootcloud;
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
                x.UseMicrosoftDependencyInjectionJobFactory();
               // x.AddJob<RootcloudJob>(rootcloud => rootcloud.WithIdentity(RootcloudJob.Key));
                x.AddJob<AdvisorGoalJob>(opts => opts.WithIdentity(AdvisorGoalJob.Key));
                x.AddJob<LongLivedOpportunitiesJob>(opts => opts.WithIdentity(LongLivedOpportunitiesJob.Key));
                x.AddJob<LogSessionJob>(opts => opts.WithIdentity(LogSessionJob.LogJobKey));
                //x.AddJob<LocationJob>(opts => opts.WithIdentity(LocationJob.Key));
               // x.AddJob<RegisterMachineryJob>(opts => opts.WithIdentity(RegisterMachineryJob.Key));

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

                //x.AddTrigger(rootcloud => rootcloud
                //.ForJob(RootcloudJob.Key)
                //.WithIdentity("rootcloud-job-trigger-xs")
                //.WithCronSchedule("0 30 07 * * ?"));

                //x.AddTrigger(rootcloud => rootcloud
                //.ForJob(RootcloudJob.Key)
                //.WithIdentity("rootcloud-job-trigger-xm")
                //.WithCronSchedule("0 30 10 * * ?"));

                //x.AddTrigger(rootcloud => rootcloud
                //.ForJob(RootcloudJob.Key)
                //.WithIdentity("rootcloud-job-trigger-md")
                //.WithCronSchedule("0 30 12 * * ?"));

                //x.AddTrigger(rootcloud => rootcloud
                //.ForJob(RootcloudJob.Key)
                //.WithIdentity("rootcloud-job-trigger-lg")
                //.WithCronSchedule("0 30 14 * * ?"));

                //x.AddTrigger(rootcloud => rootcloud
                //.ForJob(RootcloudJob.Key)
                //.WithIdentity("rootcloud-job-trigger-xl")
                //.WithCronSchedule("0 30 16 * * ?"));

                //x.AddTrigger(rootcloud => rootcloud
                //.ForJob(RootcloudJob.Key)
                //.WithIdentity("rootcloud-job-trigger-xxl")
                //.WithCronSchedule("0 30 18 * * ?"));

                //x.AddTrigger(register => register
                //.ForJob(RegisterMachineryJob.Key)
                //.WithIdentity("register-job-trigger")
                //.WithCronSchedule("0 30 01 * * ?"));

                //x.AddTrigger(location => location
                //.ForJob(LocationJob.Key)
                //.WithIdentity("location-job-trigger-large")
                //.WithCronSchedule("0 0 23 ? * MON,WED,FRI *"));
            });
            services.AddQuartzHostedService(x => x.WaitForJobsToComplete = true);
            // ---------------- End -----------------
            services.AddHttpContextAccessor();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.Configure<MetaSettings>(configuration.GetSection("MetaSettings"));
            services.Configure<MetaAdSettings>(configuration.GetSection("MetaAdSettings"));
            services.Configure<GoogleCalendarSettings>(configuration.GetSection("GoogleCalendar"));
            services.Configure<LoginRequestDto>(configuration.GetSection("Rootcloud"));
            // configure blob storage service
            services.AddScoped(_ =>
            {
                return new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage"));
            });
            services.AddTransient<INewEncryptionService, NewEncryptionService>();
            services.AddTransient<IMailService, MailService>();
            services.AddTransient<IBlobStorageService, BlobStorageService>();
            services.AddTransient<IRegisterClientService, RegisterClientService>();
            services.AddTransient<IHeaderService, HeaderService>();
            services.AddTransient<IJwtService, JwtService>();
            services.AddTransient<IAssignUserToOpportunityService, AssignUserToOpportunityService>();
            services.AddTransient<IAssignUserToProspectService, AssignUserToProspectService>();
            services.AddTransient<IMetaPostService, MetaPostService>();
            services.AddTransient<IGoogleCalendarService, GoogleCalendarService>();
            services.AddTransient<IRootcloudMachineryService, RootcloudMachineryService>();
            services.AddTransient<IRootcloudSessionService, RootcloudSessionService>();
            services.AddTransient<IRootcloudHistoricalService, RootcloudHistoricalService>();

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"])),
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
                            (path.StartsWithSegments("/hub/notification")))
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
