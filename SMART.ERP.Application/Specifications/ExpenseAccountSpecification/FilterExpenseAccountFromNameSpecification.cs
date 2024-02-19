using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.ExpenseAccountSpecification
{
    public class FilterExpenseAccountFromNameSpecification : Specification<ExpenseAccount>
    {
        public FilterExpenseAccountFromNameSpecification(string name)
        {
            Query.Where(x => x.Name == name);
        }
    }
}
