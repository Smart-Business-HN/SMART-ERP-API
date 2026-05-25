using Quartz;
using SMART.ERP.Application.Repository;
using SMART.ERP.Application.Services.AccountingPostingService;
using SMART.ERP.Application.Specifications.InvoiceSpecification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Jobs.RevenueRecognitionJob
{
    /// <summary>
    /// Job diario que recorre las facturas SaaS prepagadas con devengo vencido y dispara el
    /// reconocimiento mensual contra 4103001. Idempotente (cada mes se identifica con un Reference único),
    /// por lo que es seguro re-ejecutarlo manualmente o tras un reinicio del scheduler.
    /// </summary>
    public class RevenueRecognitionJob : IJob
    {
        public static readonly JobKey Key = new JobKey("revenue-recognition-job", "jobs");

        private readonly IRepositoryAsync<Invoice> _invoiceRepository;
        private readonly IAccountingPostingService _accountingPostingService;

        public RevenueRecognitionJob(IRepositoryAsync<Invoice> invoiceRepository, IAccountingPostingService accountingPostingService)
        {
            _invoiceRepository = invoiceRepository;
            _accountingPostingService = accountingPostingService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var ct = context.CancellationToken;
            var today = DateTime.Now;
            var pending = await _invoiceRepository.ListAsync(new FilterInvoicesDueForRecognitionSpecification(today));

            foreach (var invoice in pending)
            {
                try
                {
                    // El método maneja sus propias guard clauses (toggle apagado, mes ya posteado, fecha futura).
                    // Posibles N reconocimientos atrasados se resuelven en ejecuciones consecutivas del job.
                    var posted = true;
                    var safety = invoice.RecognitionMonths ?? 0; // cota superior por seguridad
                    while (posted && safety-- > 0)
                    {
                        posted = await _accountingPostingService.PostInvoiceRevenueRecognitionAsync(invoice.Id, ct);
                    }
                }
                catch
                {
                    // Falla en una factura no debe abortar el resto. Errores se reflejan en logs/journal entries.
                }
            }
        }
    }
}
