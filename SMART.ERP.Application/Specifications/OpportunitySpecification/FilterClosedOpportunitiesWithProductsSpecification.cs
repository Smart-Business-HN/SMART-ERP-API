using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System.Globalization;

namespace SMART.ERP.Application.Specifications.OpportunitySpecification
{
    /// <summary>
    /// Specification optimizada para obtener oportunidades cerradas con productos
    /// Incluye filtrado por fecha, tiempo (año/mes/semana/día) y branch office
    /// </summary>
    public class FilterClosedOpportunitiesWithProductsSpecification : Specification<Opportunity>
    {
        public FilterClosedOpportunitiesWithProductsSpecification(
            DateTime date,
            string? timeFilter,
            int? branchOfficeId)
        {
            // ✓ Eager loading optimizado con Include
            Query
                .Include(x => x.QuoteProducts!.Where(qp => qp.IsActive))
                    .ThenInclude(x => x.Product!)
                    .ThenInclude(x => x.SubCategory!)
                    .ThenInclude(x => x.Category)
                .Include(x => x.User)
                .Where(x => x.ClosingDate != null)
                .AsNoTracking();

            // ✓ Filtrado por tiempo en SQL
            if (!string.IsNullOrEmpty(timeFilter))
            {
                switch (timeFilter.ToLower())
                {
                    case "año":
                        Query.Where(x => x.ClosingDate!.Value.Year == date.Year);
                        break;
                    case "mes":
                        Query.Where(x => x.ClosingDate!.Value.Year == date.Year &&
                                        x.ClosingDate.Value.Month == date.Month);
                        break;
                    case "semana":
                        // Para semana, filtramos por mes (el filtrado fino se hace en memoria)
                        Query.Where(x => x.ClosingDate!.Value.Year == date.Year &&
                                        x.ClosingDate.Value.Month == date.Month);
                        break;
                    case "dia":
                        Query.Where(x => x.ClosingDate!.Value.Year == date.Year &&
                                        x.ClosingDate.Value.Month == date.Month &&
                                        x.ClosingDate.Value.Day == date.Day);
                        break;
                    default:
                        Query.Where(x => x.ClosingDate!.Value.Year == date.Year &&
                                        x.ClosingDate.Value.Month == date.Month);
                        break;
                }
            }
            else
            {
                Query.Where(x => x.ClosingDate!.Value.Year == date.Year &&
                                x.ClosingDate.Value.Month == date.Month);
            }

            // ✓ Filtrado por BranchOffice en SQL
            if (branchOfficeId.HasValue)
            {
                Query.Where(x => x.User!.BranchOfficeId == branchOfficeId.Value);
            }
        }

        /// <summary>
        /// Filtro adicional en memoria para semana (no se puede hacer eficientemente en SQL)
        /// </summary>
        public static List<Opportunity> FilterByWeek(List<Opportunity> opportunities, DateTime referenceDate)
        {
            var cal = CultureInfo.InvariantCulture.Calendar;
            var targetWeek = cal.GetWeekOfYear(referenceDate, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            return opportunities
                .Where(x => x.ClosingDate.HasValue &&
                           cal.GetWeekOfYear(x.ClosingDate.Value, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday) == targetWeek)
                .ToList();
        }
    }
}
