using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    /// <summary>
    /// Lista paginada de productos eliminados (papelera del admin).
    /// Ignora el filtro global de soft delete y filtra por IsDeleted.
    /// </summary>
    public class DeletedProductsSpecification : Specification<Product>
    {
        public DeletedProductsSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.IgnoreQueryFilters().Where(x => x.IsDeleted)
                .Include(x => x.ProductImages)
                .Skip(pageNumber * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Name.Contains(parameter) || x.Code.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Name" ? x.Name
                        : column == "Code" ? x.Code : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Name" ? x.Name
                        : column == "Code" ? x.Code : null);
                }
            }
            else
            {
                Query.OrderByDescending(x => x.DeletedAt);
            }
        }
    }

    /// <summary>
    /// Cuenta el total de productos eliminados (para la paginacion de la papelera).
    /// </summary>
    public class DeletedProductsCountSpecification : Specification<Product>
    {
        public DeletedProductsCountSpecification(string? parameter)
        {
            Query.IgnoreQueryFilters().Where(x => x.IsDeleted);
            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Name.Contains(parameter) || x.Code.Contains(parameter));
            }
        }
    }
}
