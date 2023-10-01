using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.CustomerSpecification
{
    public class BasicCustomerIncludesSpecification : Specification<Customer>
    {
        public BasicCustomerIncludesSpecification()
        {
            Query.Include(x => x.CustomerType);
        }
    }
}
