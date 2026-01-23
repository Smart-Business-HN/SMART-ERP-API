using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    /// <summary>
    /// Specification optimizada para obtener oportunidades ganadas con información del cliente
    /// Carga solo los datos necesarios con Include para evitar N+1 queries
    /// </summary>
    public class FilterWonOpportunitiesWithCustomerInfoSpecification : Specification<Opportunity>
    {
        public FilterWonOpportunitiesWithCustomerInfoSpecification()
        {
            Query
                .Where(x => x.OpportunityStep!.Name == "Ganado")
                .Include(x => x.Customer)
                .AsNoTracking();
        }
    }
}
