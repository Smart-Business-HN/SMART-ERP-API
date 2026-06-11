using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.ProspectSpecification
{
    public class FilterAndPaginationProspectSpecification : Specification<Prospect>
    {
        public FilterAndPaginationProspectSpecification(string? parameter, string? order, string? column)
        {
            // IgnoreQueryFilters: historico, debe resolver productos eliminados (soft delete).
            Query.IgnoreQueryFilters();
            Query.Include(x => x.Department).Include(x => x.ProspectStep).Include(x => x.ProspectQuoteProducts!.Where(x => x.IsActive))
                .ThenInclude(x => x.Product).Include(x => x.User).AsNoTracking();
            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Where(x => x.FullName.Contains(parameter) || x.Email!.Contains(parameter)
                || x.PhoneNumber.Contains(parameter) || x.HeadingName.Contains(parameter) || x.TypeOrigin!.Name.Contains(parameter)
                || x.SocialReasonName.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.OrderByDescending(x => column == "FullName" ? x.FullName
                    : column == "PhoneNumber" ? x.PhoneNumber : column == "Email" ? x.Email
                    : column == "Heading" ? x.HeadingName : column == "TypeOrigin" ? x.TypeOrigin!.Name
                    : column == "SocialReason" ? x.SocialReasonName : column == "ProspectStep" ? x.ProspectStepId : null);
                }
                else
                {
                    Query.OrderBy(x => column == "FullName" ? x.FullName
                    : column == "PhoneNumber" ? x.PhoneNumber : column == "Email" ? x.Email
                    : column == "Heading" ? x.HeadingName : column == "TypeOrigin" ? x.TypeOrigin!.Name
                    : column == "SocialReason" ? x.SocialReasonName : column == "ProspectStep" ? x.ProspectStepId : null);
                }
            }
        }
    }
}
