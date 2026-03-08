using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Quartz;
using SMART.ERP.Application.DTOs.Invoice;
using SMART.ERP.Application.DTOs.Notification;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.InvoiceGenerationService;
using SMART.ERP.Application.Services.SignalRHub;
using SMART.ERP.Application.Specifications.RecurringInvoiceTemplateSpecification;
using SMART.ERP.Application.Specifications.UserSpecification;
using SMART.ERP.Domain.Entities;
using SMART.ERP.Application.Features.RecurringInvoiceTemplateFeature.Commands.CreateRecurringInvoiceTemplateCommand;

namespace SMART.ERP.Application.Jobs.RecurringInvoiceJob
{
    public class RecurringInvoiceJob : IJob
    {
        public static readonly JobKey Key = new JobKey("recurring-invoice-job", "jobs");

        private readonly IRepositoryAsync<RecurringInvoiceTemplate> _templateRepositoryAsync;
        private readonly IRepositoryAsync<RecurringInvoiceLog> _logRepositoryAsync;
        private readonly IRepositoryAsync<Invoice> _invoiceRepositoryAsync;
        private readonly IRepositoryAsync<ProductSold> _productSoldRepositoryAsync;
        private readonly IRepositoryAsync<Cai> _caiRepositoryAsync;
        private readonly IRepositoryAsync<Tax> _taxRepositoryAsync;
        private readonly IRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IRepositoryAsync<User> _userRepositoryAsync;
        private readonly IRepositoryAsync<Notification> _notificationRepositoryAsync;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IInvoiceGenerationService _invoiceGenerationService;
        private readonly IMapper _mapper;

        public RecurringInvoiceJob(
            IRepositoryAsync<RecurringInvoiceTemplate> templateRepositoryAsync,
            IRepositoryAsync<RecurringInvoiceLog> logRepositoryAsync,
            IRepositoryAsync<Invoice> invoiceRepositoryAsync,
            IRepositoryAsync<ProductSold> productSoldRepositoryAsync,
            IRepositoryAsync<Cai> caiRepositoryAsync,
            IRepositoryAsync<Tax> taxRepositoryAsync,
            IRepositoryAsync<Customer> customerRepositoryAsync,
            IRepositoryAsync<User> userRepositoryAsync,
            IRepositoryAsync<Notification> notificationRepositoryAsync,
            IHubContext<NotificationHub> hubContext,
            IInvoiceGenerationService invoiceGenerationService,
            IMapper mapper)
        {
            _templateRepositoryAsync = templateRepositoryAsync;
            _logRepositoryAsync = logRepositoryAsync;
            _invoiceRepositoryAsync = invoiceRepositoryAsync;
            _productSoldRepositoryAsync = productSoldRepositoryAsync;
            _caiRepositoryAsync = caiRepositoryAsync;
            _taxRepositoryAsync = taxRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _userRepositoryAsync = userRepositoryAsync;
            _notificationRepositoryAsync = notificationRepositoryAsync;
            _hubContext = hubContext;
            _invoiceGenerationService = invoiceGenerationService;
            _mapper = mapper;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var today = DateTime.UtcNow;
                var templates = await _templateRepositoryAsync.ListAsync(new FilterActiveTemplatesDueTodaySpecification(today));

                if (templates.Count == 0)
                    return;

                var taxes = await _taxRepositoryAsync.ListAsync();
                var caiList = await _caiRepositoryAsync.ListAsync();
                var caiIssues = new List<string>();

                foreach (var template in templates)
                {
                    try
                    {
                        // Validate customer is active
                        var customer = await _customerRepositoryAsync.GetByIdAsync(template.CustomerId);
                        if (customer == null || !customer.IsActive)
                        {
                            await LogExecution(template.Id, null, false, $"Cliente {template.CustomerId} no encontrado o inactivo.");
                            continue;
                        }

                        // Find a valid CAI for the branch office
                        var validCai = caiList.FirstOrDefault(c =>
                            c.IsActive &&
                            c.AvailableInvoices > 0 &&
                            DateTime.UtcNow.Date <= c.ValidUntil &&
                            (c.IsGeneralCai || c.BranchOfficeId == template.BranchOfficeId));

                        if (validCai == null)
                        {
                            await LogExecution(template.Id, null, false, $"No hay CAI disponible para la sucursal {template.BranchOfficeId}.");
                            caiIssues.Add($"Plantilla #{template.Id} - Cliente: {customer.FullName} - Sucursal ID: {template.BranchOfficeId}");
                            continue;
                        }

                        // Create invoice from template
                        var invoiceNumber = _invoiceGenerationService.CreateInvoiceNumber(validCai);

                        var newInvoice = new Invoice
                        {
                            CaiId = validCai.Id,
                            InvoiceNumber = invoiceNumber,
                            CustomerId = template.CustomerId,
                            BranchOfficeId = template.BranchOfficeId,
                            UserId = template.UserId,
                            CreationDate = DateTime.UtcNow,
                            Observations = template.Observations,
                            TermsAndConditions = template.TermsAndConditions,
                            StatusId = template.StatusId,
                            InvoicePaymentTypeId = template.InvoicePaymentTypeId,
                            ExpectedPaymentDate = CalculateExpectedPaymentDate(template.DayOfMonth, today),
                            CreatedBy = "Sistema - Facturacion Recurrente",
                            InsertedDate = DateTime.UtcNow,
                            ProjectId = template.ProjectId,
                            RecurringInvoiceTemplateId = template.Id,
                            Exempt = 0,
                            Exonerated = 0,
                            TaxedAt15Percent = 0,
                            TaxedAt18Percent = 0,
                            Taxes15Percent = 0,
                            Taxes18Percent = 0,
                            Total = 0,
                            Outstanding = 0
                        };

                        var invoiceResponse = await _invoiceRepositoryAsync.AddAsync(newInvoice);
                        await _invoiceRepositoryAsync.SaveChangesAsync();

                        // Update CAI correlative and available invoices
                        if (validCai.AvailableInvoices < (validCai.EndCorrelative - validCai.StartCorrelative))
                        {
                            validCai.AvailableInvoices -= 1;
                            validCai.CurrentCorrelative += 1;
                        }
                        else
                        {
                            validCai.AvailableInvoices -= 1;
                        }
                        await _caiRepositoryAsync.UpdateAsync(validCai);
                        await _caiRepositoryAsync.SaveChangesAsync();

                        // Create ProductSold records from template items
                        var productsSold = new List<ProductSoldDto>();
                        if (template.Items != null && template.Items.Count > 0)
                        {
                            foreach (var item in template.Items)
                            {
                                var tax = taxes.FirstOrDefault(t => t.Id == item.TaxId);
                                var taxAmount = tax != null
                                    ? _invoiceGenerationService.TaxCalculator(item.Quantity, item.UnitPrice, tax)
                                    : 0;

                                var productSold = new ProductSold
                                {
                                    InvoiceId = invoiceResponse.Id,
                                    ProductId = item.ProductId,
                                    ProductCode = item.ProductCode,
                                    ProductDescription = item.ProductDescription,
                                    UnitPrice = item.UnitPrice,
                                    Quantity = item.Quantity,
                                    TaxId = item.TaxId,
                                    Taxes = taxAmount,
                                    TotalLine = taxAmount + (item.Quantity * item.UnitPrice)
                                };
                                var productSoldResponse = await _productSoldRepositoryAsync.AddAsync(productSold);
                                await _productSoldRepositoryAsync.SaveChangesAsync();

                                productsSold.Add(new ProductSoldDto
                                {
                                    Id = productSoldResponse.Id,
                                    InvoiceId = invoiceResponse.Id,
                                    ProductId = item.ProductId,
                                    ProductCode = item.ProductCode,
                                    ProductDescription = item.ProductDescription,
                                    UnitPrice = item.UnitPrice,
                                    Quantity = item.Quantity,
                                    TaxId = item.TaxId,
                                    Taxes = taxAmount,
                                    TotalLine = taxAmount + (item.Quantity * item.UnitPrice)
                                });
                            }

                            // Calculate tax totals
                            var tax0 = taxes.Find(x => x.Rate == 0);
                            var tax15 = taxes.Find(x => x.Rate == 15);
                            var tax18 = taxes.Find(x => x.Rate == 18);

                            if (tax0 != null) invoiceResponse.Exempt = _invoiceGenerationService.CalculateGravableValue(productsSold, tax0);
                            if (tax15 != null)
                            {
                                invoiceResponse.TaxedAt15Percent = _invoiceGenerationService.CalculateGravableValue(productsSold, tax15);
                                invoiceResponse.Taxes15Percent = _invoiceGenerationService.CalculateTaxesValue(productsSold, tax15);
                            }
                            if (tax18 != null)
                            {
                                invoiceResponse.TaxedAt18Percent = _invoiceGenerationService.CalculateGravableValue(productsSold, tax18);
                                invoiceResponse.Taxes18Percent = _invoiceGenerationService.CalculateTaxesValue(productsSold, tax18);
                            }

                            invoiceResponse.Total = invoiceResponse.TaxedAt15Percent + invoiceResponse.TaxedAt18Percent
                                + invoiceResponse.Taxes15Percent + invoiceResponse.Taxes18Percent + invoiceResponse.Exempt;
                            invoiceResponse.Outstanding = invoiceResponse.Total;

                            await _invoiceRepositoryAsync.UpdateAsync(invoiceResponse);
                            await _invoiceRepositoryAsync.SaveChangesAsync();
                        }

                        // Log success
                        await LogExecution(template.Id, invoiceResponse.Id, true, null);

                        // Update template dates
                        template.LastGeneratedDate = DateTime.UtcNow;
                        template.NextGenerationDate = CreateRecurringInvoiceTemplateCommandHandler
                            .CalculateNextGenerationDate(template.DayOfMonth, DateTime.UtcNow);
                        await _templateRepositoryAsync.UpdateAsync(template);
                        await _templateRepositoryAsync.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        await LogExecution(template.Id, null, false, ex.Message.Length > 500 ? ex.Message[..500] : ex.Message);
                    }
                }

                // Notify admins about CAI issues
                if (caiIssues.Count > 0)
                {
                    await NotifyAdminsAboutCaiIssues(caiIssues);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task LogExecution(int templateId, int? invoiceId, bool succeeded, string? errorMessage)
        {
            var log = new RecurringInvoiceLog
            {
                RecurringInvoiceTemplateId = templateId,
                GeneratedInvoiceId = invoiceId,
                ExecutionDate = DateTime.UtcNow,
                Succeeded = succeeded,
                ErrorMessage = errorMessage
            };
            await _logRepositoryAsync.AddAsync(log);
            await _logRepositoryAsync.SaveChangesAsync();
        }

        private async Task NotifyAdminsAboutCaiIssues(List<string> caiIssues)
        {
            var managers = await _userRepositoryAsync.ListAsync(new FilterUserByRoleSpecification("Manager", null));
            var admins = await _userRepositoryAsync.ListAsync(new FilterUserByRoleSpecification("Admin", null));
            var recipients = managers.Concat(admins).DistinctBy(u => u.Id).ToList();

            var issueDetails = string.Join("<br/>", caiIssues);
            foreach (var recipient in recipients)
            {
                var notification = new Notification
                {
                    Title = "Alerta: CAI no disponible para facturas recurrentes",
                    Icon = "mat_outline:warning_amber",
                    Description = $"No se pudieron generar {caiIssues.Count} factura(s) recurrente(s) por falta de CAI disponible:<br/>{issueDetails}",
                    Time = DateTime.Now,
                    UseRouter = true,
                    Link = "/accounting/recurring-invoices",
                    Read = false,
                    UserId = recipient.Id
                };
                var response = await _notificationRepositoryAsync.AddAsync(notification);
                await _notificationRepositoryAsync.SaveChangesAsync();

                var dto = _mapper.Map<NotificationDto>(response);
                await _hubContext.Clients.User(recipient.FullName).SendAsync("NewNotification", dto);
            }
        }

        private static DateOnly CalculateExpectedPaymentDate(int dayOfMonth, DateTime today)
        {
            if (dayOfMonth == -1)
            {
                var lastDay = DateTime.DaysInMonth(today.Year, today.Month);
                return new DateOnly(today.Year, today.Month, lastDay);
            }
            var day = Math.Min(dayOfMonth, DateTime.DaysInMonth(today.Year, today.Month));
            return new DateOnly(today.Year, today.Month, day);
        }
    }
}
