using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.ERP.Domain.Entities
{
    public class IncomeAccount
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public int MajorIncomeAccountId { get; set; }
        public MajorIncomeAccount? MajorIncomeAccount { get; set;}
    }
}
