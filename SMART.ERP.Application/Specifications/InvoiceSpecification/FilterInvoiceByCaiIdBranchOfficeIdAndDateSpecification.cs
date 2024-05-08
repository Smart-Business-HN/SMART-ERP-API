using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InvoiceSpecification
{
    public class FilterInvoiceByCaiIdBranchOfficeIdAndDateSpecification : Specification<Invoice>
    {
        public FilterInvoiceByCaiIdBranchOfficeIdAndDateSpecification(string caiIdentificator, int branchOfficeId, DateOnly date)
        {
            Query.Include(x => x.BillPayments).ThenInclude(x => x.TypeOfPaymentMethod).Where(x => x.Cai.Identificator == caiIdentificator && x.BranchOfficeId == branchOfficeId && x.CreationDate.Date == date.ToDateTime(TimeOnly.MinValue));
        }
    }
}
