using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using SMART.ERP.Application.DTOs.Mail;
using SMART.ERP.Application.DTOs.Notification;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.MailService;
using SMART.ERP.Application.Services.RepricingEngine;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Specifications.RepricingSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Domain.Enums;
using SMART.ERP.Domain.Settings;
using System.Globalization;
using System.Net;
using System.Text;

namespace SMART.ERP.Application.Jobs.CompetitorPriceMonitorJob
{
    /// <summary>
    /// Monitorea diariamente los precios de competencia y re-fija los precios de la tienda. Aísla cada
    /// producto en su propio try/catch para que un fallo no aborte la corrida; alerta a admins sobre
    /// pisos tocados, fallos de lectura o caídas bloqueadas por la guarda, y envía un correo de estatus.
    /// </summary>
    public class CompetitorPriceMonitorJob : IJob
    {
        public static readonly JobKey Key = new("competitor-price-monitor-job", "jobs");

        private readonly IReadRepositoryAsync<CompetitorSource> _sourceRepo;
        private readonly IReadRepositoryAsync<RepricingSettings> _settingsRepo;
        private readonly IReadRepositoryAsync<Product> _productRepo;
        private readonly IRepricingEngineService _engine;
        private readonly IEcommerceRevalidationService _revalidation;
        private readonly IMailService _mailService;
        private readonly IRepositoryAsync<User> _userRepo;
        private readonly IRepositoryAsync<Notification> _notificationRepo;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IMapper _mapper;
        private readonly RepricingScraperSettings _scraperSettings;
        private readonly ILogger<CompetitorPriceMonitorJob> _logger;

        public CompetitorPriceMonitorJob(
            IReadRepositoryAsync<CompetitorSource> sourceRepo,
            IReadRepositoryAsync<RepricingSettings> settingsRepo,
            IReadRepositoryAsync<Product> productRepo,
            IRepricingEngineService engine,
            IEcommerceRevalidationService revalidation,
            IMailService mailService,
            IRepositoryAsync<User> userRepo,
            IRepositoryAsync<Notification> notificationRepo,
            IHubContext<NotificationHub> hubContext,
            IMapper mapper,
            IOptions<RepricingScraperSettings> scraperSettings,
            ILogger<CompetitorPriceMonitorJob> logger)
        {
            _sourceRepo = sourceRepo;
            _settingsRepo = settingsRepo;
            _productRepo = productRepo;
            _engine = engine;
            _revalidation = revalidation;
            _mailService = mailService;
            _userRepo = userRepo;
            _notificationRepo = notificationRepo;
            _hubContext = hubContext;
            _mapper = mapper;
            _scraperSettings = scraperSettings.Value;
            _logger = logger;
        }

        /// <summary>Resultado por producto en la corrida: el log generado, o un error inesperado.</summary>
        private sealed record ProductResult(int ProductId, PriceChangeLog? Log, string? Error);

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
                var results = new List<ProductResult>();

                foreach (var productId in productIds)
                {
                    try
                    {
                        var log = await _engine.EvaluateAndApplyAsync(productId, "Sistema - Repricing", ct);
                        if (log is null)
                            continue; // sin fuentes habilitadas / producto excluido → no se reporta

                        if (log.Applied)
                            anyApplied = true;

                        if (log.Status == PriceChangeStatus.ScrapeFailed)
                            alerts.Add($"Producto #{productId}: no se pudo leer ningún competidor.");
                        else if (log.Status == PriceChangeStatus.RejectedByGuard)
                            alerts.Add($"Producto #{productId}: caída de precio bloqueada por la guarda — {log.Reason}");
                        else if (log.FloorHit)
                            alerts.Add($"Producto #{productId}: precio en el piso de margen (no se pudo superar al competidor sin perder margen).");

                        results.Add(new ProductResult(productId, log, null));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al repreciar el producto {ProductId}", productId);
                        var msg = ex.Message.Length > 200 ? ex.Message[..200] : ex.Message;
                        alerts.Add($"Producto #{productId}: error inesperado — {msg}");
                        results.Add(new ProductResult(productId, null, msg));
                    }
                }

                if (anyApplied)
                    await _revalidation.RevalidateStoreAsync(ct);

                if (alerts.Count > 0)
                    await NotifyAdmins(alerts);

                // Reporte por correo (resumen completo de cada corrida).
                if (results.Count > 0 && !string.IsNullOrWhiteSpace(_scraperSettings.ReportEmailTo))
                    await SendStatusEmail(results, ct);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task SendStatusEmail(List<ProductResult> results, CancellationToken ct)
        {
            try
            {
                var ids = results.Select(r => r.ProductId).Distinct().ToList();
                var products = await _productRepo.ListAsync(new ProductsByIdsSpecification(ids), ct);
                var map = products.ToDictionary(p => p.Id, p => (p.Code, p.Name));

                var mail = new MailRequestDto
                {
                    ToEmail = _scraperSettings.ReportEmailTo!,
                    Subject = $"Reporte de re-fijación de precios — {DateTime.Now:dd/MM/yyyy HH:mm}",
                    Body = BuildReportHtml(results, map)
                };
                await _mailService.SendEmailAsync(mail);
            }
            catch (Exception ex)
            {
                // El envío no debe abortar la corrida.
                _logger.LogError(ex, "No se pudo enviar el correo de estatus de re-fijación.");
            }
        }

        private const string ThStyle = "style=\"text-align:left;padding:6px 10px;border:1px solid #ddd;background:#f5f5f5;\"";
        private const string TdStyle = "style=\"padding:6px 10px;border:1px solid #ddd;\"";

        private static string BuildReportHtml(List<ProductResult> results, IReadOnlyDictionary<int, (string Code, string Name)> map)
        {
            string Name(int id) => map.TryGetValue(id, out var p)
                ? WebUtility.HtmlEncode($"{p.Code} — {p.Name}")
                : $"#{id}";
            string Money(decimal? v) => v.HasValue ? $"L {v.Value.ToString("N2", CultureInfo.InvariantCulture)}" : "—";

            var applied = results.Where(r => r.Log is { Applied: true }).ToList();
            var others = results.Where(r => r.Log is not { Applied: true }).ToList();
            var skipped = others.Count(r => r.Error is null && r.Log?.Status == PriceChangeStatus.Skipped);
            var issues = others.Count - skipped;

            var sb = new StringBuilder();
            sb.Append("<div style=\"font-family:Arial,Helvetica,sans-serif;color:#222;\">");
            sb.Append("<h2 style=\"margin:0 0 4px;\">Reporte de re-fijación de precios</h2>");
            sb.Append($"<p style=\"margin:0 0 16px;color:#555;\">{DateTime.Now:dd/MM/yyyy HH:mm} &middot; " +
                      $"{results.Count} evaluados &middot; {applied.Count} actualizados &middot; " +
                      $"{skipped} sin cambio &middot; {issues} con alerta/fallo</p>");

            // Actualizados
            sb.Append("<h3 style=\"margin:16px 0 8px;\">Actualizados</h3>");
            if (applied.Count == 0)
            {
                sb.Append("<p style=\"color:#777;\">Ninguno en esta corrida.</p>");
            }
            else
            {
                sb.Append(TableOpen("Producto", "Precio anterior", "Precio nuevo", "Mín. competencia"));
                foreach (var r in applied)
                {
                    var l = r.Log!;
                    sb.Append(Row(Name(r.ProductId), Money(l.OldPrice), Money(l.AppliedPrice ?? l.ProposedPrice), Money(l.MinCompetitorPrice)));
                }
                sb.Append("</table>");
            }

            // No actualizados
            sb.Append("<h3 style=\"margin:20px 0 8px;\">No actualizados</h3>");
            if (others.Count == 0)
            {
                sb.Append("<p style=\"color:#777;\">Ninguno.</p>");
            }
            else
            {
                sb.Append(TableOpen("Producto", "Estado", "Detalle"));
                foreach (var r in others)
                {
                    var status = r.Error is not null ? "Error" : StatusLabel(r.Log!.Status);
                    var detail = WebUtility.HtmlEncode(r.Error ?? r.Log?.Reason ?? "");
                    sb.Append(Row(Name(r.ProductId), status, detail));
                }
                sb.Append("</table>");
            }

            sb.Append("</div>");
            return sb.ToString();
        }

        private static string TableOpen(params string[] headers)
        {
            var sb = new StringBuilder("<table style=\"border-collapse:collapse;width:100%;font-size:13px;\"><tr>");
            foreach (var h in headers) sb.Append($"<th {ThStyle}>{h}</th>");
            sb.Append("</tr>");
            return sb.ToString();
        }

        private static string Row(params string[] cells)
        {
            var sb = new StringBuilder("<tr>");
            foreach (var c in cells) sb.Append($"<td {TdStyle}>{c}</td>");
            sb.Append("</tr>");
            return sb.ToString();
        }

        private static string StatusLabel(PriceChangeStatus s) => s switch
        {
            PriceChangeStatus.Applied => "Aplicado",
            PriceChangeStatus.Skipped => "Sin cambio",
            PriceChangeStatus.FloorHeld => "En el piso",
            PriceChangeStatus.RejectedByGuard => "Bloqueado por guarda",
            PriceChangeStatus.ScrapeFailed => "Fallo de lectura",
            PriceChangeStatus.AwaitingApproval => "Pendiente de aprobación",
            _ => s.ToString()
        };

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
