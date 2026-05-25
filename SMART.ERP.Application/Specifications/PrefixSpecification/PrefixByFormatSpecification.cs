using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.PrefixSpecification
{
    public class PrefixByFormatSpecification : Specification<Prefix>
    {
        public PrefixByFormatSpecification(string format)
        {
            Query.Where(x => x.Format == format).AsNoTracking();
        }
    }
}
