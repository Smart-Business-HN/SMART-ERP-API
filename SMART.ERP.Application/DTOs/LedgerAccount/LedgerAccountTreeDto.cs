using SMART.ERP.Domain.Enums;

namespace SMART.ERP.Application.DTOs.LedgerAccount
{
    public class LedgerAccountTreeDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public AccountType AccountType { get; set; }
        public string AccountTypeName { get; set; } = null!;
        public AccountLevel Level { get; set; }
        public NormalBalanceSide NormalBalanceSide { get; set; }
        public int? ParentId { get; set; }
        public bool IsPostable { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystem { get; set; }
        public List<LedgerAccountTreeDto> Children { get; set; } = new();
    }
}
