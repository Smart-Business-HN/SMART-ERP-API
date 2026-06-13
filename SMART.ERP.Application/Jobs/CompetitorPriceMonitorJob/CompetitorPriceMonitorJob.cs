using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Quartz;
using SMART.ERP.Application.DTOs.Notification;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.RepricingEngine;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Specifications.RepricingSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.Jobs.CompetitorPriceMonitorJob
{
    /// <summary>
    /// Monitorea diariamente los precios de competencia y re-fija los precios de la tienda. Aísla cada
    /// producto en su propio try/catch para que un fallo no aborte la corrida; alerta a admins sobre
    /// pisos tocados, fallos de lectura o caídas bloqueadas por la guarda.
    /// </summary>
    public class CompetitorPriceMonitorJob : IJob
    {
        public static readonly JobKey Key = new("competitor-price-monitor-job", "jobs");

        private readonly IReadRepositoryAsync<CompetitorSource> _sourceRepo;
        private readonly IReadRepositoryAsync<RepricingSettings> _settingsRepo;
        private readonly IRepricingEngineService _engine;
        private readonly IEcommerceRevalidationService _revalidation;
        private readonly IRepositoryAsync<User> _userRepo;
        private readonly IRepositoryAsync<Notification> _notificationRepo;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CompetitorPriceMonitorJob> _logger;

        public CompetitorPriceMonitorJob(
            IReadRepositoryAsync<CompetitorSource> sourceRepo,
            IReadRepositoryAsync<RepricingSettings> settingsRepo,
            IRepricingEngineService engine,
            IEcommerceRevalidationService revalidation,
            IRepositoryAsync<User> userRepo,
            IRepositoryAsync<Notification> notificationRepo,
            IHubContext<NotificationHub> hubContext,
            IMapper mapper,
            ILogger<CompetitorPriceMonitorJob> logger)
        {
            _sourceRepo = sourceRepo;
            _settingsRepo = settingsRepo;
            _engine = engine;
            _revalidation = revalidation;
            _userRepo = userRepo;
            _notificationRepo = notificationRepo;
            _hubContext = hubContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var ct = context.CancellationToken;
            try
            {
                var settings = await _settingsRepo.FirstOrDefaultAsync(new RepricingSettingsSingletonSpecification(), ct);
                if (settings is null || !settings.MonitoringEnabled)
                    return;

                var sources = await _sourceRepo.ListAsync(new ProductsWithEnabledSourcesSpecification(), ct);
                var productIds = sources.Select(s => s.ProductId).Distinct().ToList();
                if (productIds.Count == 0)
                    return;

                var anyApplied = false;
                var alerts = new List<string>();

                foreach (var productId in productIds)
                {
                    try
                    {
                        var log = await _engine.EvaluateAndApplyAsync(productId, "Sistema - Repricing", ct);
                        if (log is null)
                            continue;

                        if (log.Applied)
                            anyApplied = true;

                        if (log.Status == PriceChangeStatus.ScrapeFailed)
                            alerts.Add($"Producto #{productId}: no se pudo leer ningún competidor.");
                        else if (log.Status == PriceChangeStatus.RejectedByGuard)
                            alerts.Add($"Producto #{productId}: caída de precio bloqueada por la guarda — {log.Reason}");
                        else if (log.FloorHit)
                            alerts.Add($"Producto #{productId}: precio en el piso de margen (no se pudo superar al competidor sin perder margen).");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al repreciar el producto {ProductId}", productId);
                        var msg = ex.Message.Length > 200 ? ex.Message[..200] : ex.Message;
                        alerts.Add($"Producto #{productId}: error inesperado — {msg}");
                    }
                }

                if (anyApplied)
                    await _revalidation.RevalidateStoreAsync(ct);

                if (alerts.Count > 0)
                    await NotifyAdmins(alerts);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task NotifyAdmins(List<string> alerts)
        {
            var managers = await _userRepo.ListAsync(new FilterUserByRoleSpecification("Manager", null));
            var admins = await _userRepo.ListAsync(new FilterUserByRoleSpecification("Admin", null));
            var recipients = managers.Concat(admins).DistinctBy(u => u.Id).ToList();

            // Notification.Description es varchar(150): mantener el resumen corto (el detalle va en la bitácora).
            var description = $"{alerts.Count} aviso(s) de re-fijación. Revisa la pantalla de monitoreo de precios.";
            if (description.Length > 150) description = description[..150];

            foreach (var recipient in recipients)
            {
                var notification = new Notification
                {
                    Title = "Alerta: precios de competencia",
                    Icon = "mat_outline:price_change",
                    Description = description,
                    Time = DateTime.Now,
                    UseRouter = true,
                    Link = "/inventory/repricing",
                    Read = false,
                    UserId = recipient.Id
                };
                var response = await _notificationRepo.AddAsync(notification);
                await _notificationRepo.SaveChangesAsync();

                var dto = _mapper.Map<NotificationDto>(response);
                await _hubContext.Clients.User(recipient.FullName).SendAsync("NewNotification", dto);
            }
        }
    }
}
