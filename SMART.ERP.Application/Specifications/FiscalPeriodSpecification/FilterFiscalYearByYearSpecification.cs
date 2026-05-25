using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.FiscalPeriodSpecification
{
    public class FilterFiscalYearByYearSpecification : Specification<FiscalYear>
    {
        public FilterFiscalYearByYearSpecification(int year)
        {
            Query.Where(x => x.Year == year).AsNoTracking();
        }
    }
}
