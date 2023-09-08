using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PrefixSpecification
{
    public class PrefixByInternalDocumentIdSpecification : Specification<Prefix>
    {
        public PrefixByInternalDocumentIdSpecification(int internalDocumentId)
        {
            Query.Include(x=>x.InternalDocument).Where(x=>x.InternalDocumentId == internalDocumentId);
        }
    }
}
