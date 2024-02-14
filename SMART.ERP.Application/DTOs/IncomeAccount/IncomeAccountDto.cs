using SMART.ERP.Application.DTOs.MajorIncomeAccount;

namespace SMART.ERP.Application.DTOs.IncomeAccount
{
    public class IncomeAccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string AccountNumber { get; set; } = null!;
        public int MajorIncomeAccountId { get; set; }
        public MajorIncomeAccountDto? MajorIncomeAccount { get; set; }
    }
}
