using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProductSpecification
{
    /// <summary>
    /// Resuelve un producto eliminado por Id (ignora el filtro global de soft delete).
    /// Lo usa el comando Restore, ya que GetByIdAsync no ve productos eliminados.
    /// </summary>
    public class DeletedProductByIdSpecification : Specification<Product>
    {
        public DeletedProductByIdSpecification(int id)
        {
            Query.IgnoreQueryFilters().Where(x => x.Id == id && x.IsDeleted);
        }
    }
}
