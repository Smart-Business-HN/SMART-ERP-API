using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    /// <summary>
    /// Specification optimizada para obtener oportunidades ganadas filtradas por año y categoría
    /// Evita N+1 queries cargando solo los datos necesarios con Include
    /// </summary>
    public class FilterWonOpportunitiesByYearAndCategorySpecification : Specification<Opportunity>
    {
        public FilterWonOpportunitiesByYearAndCategorySpecification(int year, int? categoryId = null)
        {
            Query
                .Where(x => x.OpportunityStep!.Name == "Ganado"
                         && x.ClosingDate.HasValue
                         && x.ClosingDate.Value.Year == year)
                .Include(x => x.QuoteProducts!)
                    .ThenInclude(x => x.Product!)
                    .ThenInclude(x => x.SubCategory!)
                    .ThenInclude(x => x.Category)
                .AsNoTracking();

            // Si se especifica categoría, filtrar productos por categoría
            if (categoryId.HasValue)
            {
                Query.Where(x => x.QuoteProducts!.Any(qp =>
                    qp.Product!.SubCategory!.CategoryId == categoryId.Value));
            }
        }
    }
}
