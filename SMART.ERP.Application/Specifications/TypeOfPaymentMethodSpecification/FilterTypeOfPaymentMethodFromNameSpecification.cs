using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.TypeOfPaymentMethodSpecification
{
    public class FilterTypeOfPaymentMethodFromNameSpecification : Specification<TypeOfPaymentMethod>
    {
        public FilterTypeOfPaymentMethodFromNameSpecification(string param)
        {
            Query.Where(x => x.Name.ToLower() == param.ToLower());
        }
    }
}
