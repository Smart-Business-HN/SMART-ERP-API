using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.MajorExpenseAccountSpecification
{
    public class FilterMajorExpenseAccountFromNameSpecification : Specification<MajorExpenseAccount>
    {
        public FilterMajorExpenseAccountFromNameSpecification(string name)
        {
            Query.Where(x => x.Name == name);
        }
    }
}
