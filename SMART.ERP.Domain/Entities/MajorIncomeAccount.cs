using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class MajorIncomeAccount
    {
        public int Id { get; init; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        public List<IncomeAccount>? IncomeAccounts { get; set; }
    }
}
