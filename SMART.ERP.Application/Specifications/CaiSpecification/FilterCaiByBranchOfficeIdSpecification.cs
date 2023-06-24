using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.CaiSpecification
{
    public class FilterCaiByBranchOfficeIdSpecification : Specification<Cai>
    {
        public FilterCaiByBranchOfficeIdSpecification(int branchOfficeId)
        {
            Query.Where(x=>x.BranchOfficeId == branchOfficeId);
        }
    }
}
