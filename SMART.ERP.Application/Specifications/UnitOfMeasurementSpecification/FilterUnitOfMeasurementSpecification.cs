using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.UnitOfMeasurementSpecification
{
    public class FilterUnitOfMeasurementSpecification : Specification<UnitOfMeasurement>
    {
        public FilterUnitOfMeasurementSpecification(string filter, int? id)
        {
            if (id != null) Query.Where(x => (x.Name == filter || x.Abreviation == filter) && x.Id != id).AsNoTracking();
            else Query.Where(x => x.Name == filter || x.Abreviation == filter).AsNoTracking();
        }
    }
}
