using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.FiscalPeriodSpecification
{
    public class AllFiscalYearsWithPeriodsSpecification : Specification<FiscalYear>
    {
        public AllFiscalYearsWithPeriodsSpecification()
        {
            Query.Include(x => x.Periods!.OrderBy(p => p.PeriodNumber))
                 .OrderByDescending(x => x.Year)
                 .AsNoTracking();
        }
    }
}
