using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.PrefixSpecification
{
    public class FilterAndPaginationPrefixSpecification : Specification<Prefix>
    {
        public FilterAndPaginationPrefixSpecification(string? parameter, int pageNumber,
            int pageSize, string? order, string? column)
        {
            Query.Include(x=>x.InternalDocument).Skip((pageNumber) * pageSize).Take(pageSize).AsNoTracking();

            if (!string.IsNullOrEmpty(parameter))
            {
                Query.Include(x=> x.InternalDocument).Where(x => x.Format.Contains(parameter));
            }

            if (!string.IsNullOrEmpty(order) && !string.IsNullOrEmpty(column))
            {
                if (order == "desc")
                {
                    Query.Include(x=>x.InternalDocument).OrderByDescending(x => column == "Name" ? x.Format : null);
                }
                else
                {
                    Query.Include(x => x.InternalDocument).OrderBy(x => column == "Name" ? x.Format : null);
                }
            }
        }
    }
}
