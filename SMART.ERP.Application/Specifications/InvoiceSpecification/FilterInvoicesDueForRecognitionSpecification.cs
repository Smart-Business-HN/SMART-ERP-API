using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    /// <summary>
    /// Facturas marcadas como suscripción prepagada cuyo próximo devengo ya venció (NextRecognitionDate &lt;= today)
    /// y aún les quedan meses por reconocer (MonthsRecognized &lt; RecognitionMonths).
    /// </summary>
    public class FilterInvoicesDueForRecognitionSpecification : Specification<Invoice>
    {
        public FilterInvoicesDueForRecognitionSpecification(DateTime today)
        {
            var cutoff = today.Date.AddDays(1).AddTicks(-1); // incluye todo el día actual
            Query.Where(x =>
                x.IsDeferredRevenue
                && x.RecognitionMonths != null
                && x.MonthsRecognized < x.RecognitionMonths
                && x.NextRecognitionDate != null
                && x.NextRecognitionDate <= cutoff
            ).OrderBy(x => x.NextRecognitionDate);
        }
    }
}
