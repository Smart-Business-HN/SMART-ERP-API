using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public class FilterAndPaginationProductSpecification : Specification<Product>
    {
        public FilterAndPaginationProductSpecification(string? parameter, int pageNumber, int pageSize,
            string? order, string? column)
        {
            Query.Include(x => x.SubCategory).Include(x => x.Status).Include(x => x.Brand).Include(x => x.Provider).Include(x => x.Tax)
                .Include(x => x.ProductImages).Include(x => x.ProductDataSheets!).ThenInclude(x => x.DataSheet)
                .Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.Name.Contains(parameter) || x.Code.Contains(parameter)
                || x.SubCategory!.Name.Contains(parameter) || x.Brand!.Name.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "Name" ? x.Name
                    : column == "Code" ? x.Code : column == "SubCategory" ? x.SubCategory!.Name
                    : column == "Brand" ? x.Brand!.Name : null);
                }
                else
                {
                    Query.OrderBy(x => column == "Name" ? x.Name
                    : column == "Code" ? x.Code : column == "SubCategory" ? x.SubCategory!.Name
                    : column == "Brand" ? x.Brand!.Name : null);
                }
            }
        }
    }
}
