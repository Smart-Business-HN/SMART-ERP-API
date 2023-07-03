using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.TaxSpecification
{
    public class FilterTaxByNameSpecification : Specification<Tax>
    {
        public FilterTaxByNameSpecification( string filter, int? id) {
            if(id != null)
            {
                Query.Where(x => x.Name == filter && x.Id != id).AsNoTracking();
            }
            else
            {
                Query.Where(x => x.Name == filter).AsNoTracking();
            }
        }
    }
}
