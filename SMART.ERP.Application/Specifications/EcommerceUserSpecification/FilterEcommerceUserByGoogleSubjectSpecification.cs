using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.EcommerceUserSpecification;

/// <summary>
/// Busca por el claim "sub" de Google. Se prefiere sobre el correo porque el sub
/// es estable, mientras que el correo puede cambiar en cuentas de Google Workspace.
/// </summary>
public sealed class FilterEcommerceUserByGoogleSubjectSpecification : Specification<EcommerceUser>
{
    public FilterEcommerceUserByGoogleSubjectSpecification(string googleSubjectId)
    {
        Query.Include(x => x.CustomerType);
        Query.Include(x => x.Department);
        Query.Include(x => x.Carts);
        Query.Where(x => x.GoogleSubjectId == googleSubjectId);
    }
}
