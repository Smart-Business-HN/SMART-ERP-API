using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.InventoryMovementSpecification
{
    public class HasMovementsForProductSpecification : Specification<InventoryMovement>
    {
        public HasMovementsForProductSpecification(int productId)
        {
            Query.Where(m => m.ProductId == productId).AsNoTracking();
        }
    }
}
