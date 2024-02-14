using SMART.ERP.Application.DTOs.IncomeAccount;

namespace SMART.ERP.Application.DTOs.MajorIncomeAccount
{
    public class MajorIncomeAccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<IncomeAccountDto>? IncomeAccounts { get; set; }
    }
}
