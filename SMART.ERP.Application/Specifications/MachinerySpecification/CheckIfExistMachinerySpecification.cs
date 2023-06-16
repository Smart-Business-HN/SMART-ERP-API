using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.MachinerySpecification
{
    public class CheckIfExistMachinerySpecification : Specification<Machinery>
    {
        public CheckIfExistMachinerySpecification(string serialNum)
        {
            Query.AsNoTracking().Where(s => s.SerialNum == serialNum);
        }
    }
}
