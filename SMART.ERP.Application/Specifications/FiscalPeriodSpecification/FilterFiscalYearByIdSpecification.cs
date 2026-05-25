using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.FiscalPeriodSpecification
{
    public class FilterFiscalYearByIdSpecification : Specification<FiscalYear>
    {
        public FilterFiscalYearByIdSpecification(int id, bool asTracking = false)
        {
            Query.Include(x => x.Periods).Where(x => x.Id == id);
            if (!asTracking)
                Query.AsNoTracking();
        }
    }
}
