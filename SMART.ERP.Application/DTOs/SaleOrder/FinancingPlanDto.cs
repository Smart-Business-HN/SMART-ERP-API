using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.DTOs.SaleOrder
{
    public class FinancingPlanDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ProviderId { get; set; }
        public ProviderDto? Provider { get; set; }
        public bool IsActive { get; set; }
    }
}
