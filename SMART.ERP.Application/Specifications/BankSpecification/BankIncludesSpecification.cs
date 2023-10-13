using Ardalis.Specification;
using SMART.ERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Application.Specifications.BankSpecification
{
    public class BankIncludesSpecification : Specification<Bank>
    {
        public BankIncludesSpecification(int id) {
            Query.Include(x => x.InternalBankAccounts).Where(x => x.Id == id);
        }
    }
}
