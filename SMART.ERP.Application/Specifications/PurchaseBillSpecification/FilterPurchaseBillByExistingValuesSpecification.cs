using Ardalis.Specification;
using SMART.ERP.Application.Features.PurchaseBillFeature.Commands.CreatePurchaseBillCommand;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PurchaseBillSpecification
{
    public class FilterPurchaseBillByExistingValuesSpecification : Specification<PurchaseBill>
    {
        public FilterPurchaseBillByExistingValuesSpecification(CreatePurchaseBillCommand request) {
            Query.Where(x=> (x.Cai == request.Cai && x.InvoiceNumber == request.InvoiceNumber)
                            ||(x.ProviderId == request.ProviderId && x.InvoiceNumber == request.InvoiceNumber));
        }
    }
}
