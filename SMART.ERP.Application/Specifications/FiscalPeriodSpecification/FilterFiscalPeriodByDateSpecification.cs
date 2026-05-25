using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.FiscalPeriodSpecification
{
    /// <summary>El período fiscal que contiene la fecha dada.</summary>
    public class FilterFiscalPeriodByDateSpecification : Specification<FiscalPeriod>
    {
        public FilterFiscalPeriodByDateSpecification(DateTime date)
        {
            var day = date.Date;
            Query.Include(x => x.FiscalYear)
                 .Where(x => x.StartDate <= day && x.EndDate >= day);
        }
    }
}
