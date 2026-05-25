using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.FiscalPeriodSpecification
{
    public class FilterFiscalPeriodsByYearSpecification : Specification<FiscalPeriod>
    {
        public FilterFiscalPeriodsByYearSpecification(int fiscalYearId)
        {
            Query.Where(x => x.FiscalYearId == fiscalYearId).OrderBy(x => x.PeriodNumber);
        }
    }
}
