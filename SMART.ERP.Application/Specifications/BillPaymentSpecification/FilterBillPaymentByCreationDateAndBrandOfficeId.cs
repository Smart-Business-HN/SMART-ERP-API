using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.BillPaymentSpecification
{
    public class FilterBillPaymentByCreationDateAndBrandOfficeId : Specification<BillPayment>
    {
        public FilterBillPaymentByCreationDateAndBrandOfficeId(int caiId, int branchOfficeId, DateOnly creationDate)
        {
            Query.Include(x => x.Invoice).ThenInclude(x => x!.InvoicePaymentType)
                .Include(x => x.TypeOfPaymentMethod)
                .Where(x => x.Invoice!.BranchOfficeId == branchOfficeId && x.Invoice.CaiId == caiId && x.Date.Date == creationDate.ToDateTime(TimeOnly.MinValue).Date);
        }
    }
}
