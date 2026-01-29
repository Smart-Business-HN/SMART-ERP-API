using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class IncomeAccount
    {
        public int Id { get; init; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        [MaxLength(50)]
        public string AccountNumber { get; set; } = null!;
        public int MajorIncomeAccountId { get; set; }
        public MajorIncomeAccount? MajorIncomeAccount { get; set;}
    }
}
