using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    public sealed class ProductSearchSuggestionsSpecification : Specification<Product>
    {
        public ProductSearchSuggestionsSpecification(string searchTerm, string fieldType, int limit)
        {
            Query.Where(x => x.ShowInEcommerce && x.IsActive)
                .AsNoTracking();

            switch (fieldType.ToLower())
            {
                case "name":
                    Query.Where(x => x.Name.ToLower().Contains(searchTerm));
                    break;
                case "brand":
                    Query.Where(x => x.Brand!.Name.ToLower().Contains(searchTerm));
                    break;
                case "category":
                    Query.Where(x => x.SubCategory!.Category!.Name.ToLower().Contains(searchTerm));
                    break;
            }

            Query.Take(limit);
        }
    }
}

