using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.DTOs.Opportunity;
using SMART.ERP.Application.DTOs.Status;

namespace SMART.ERP.Application.DTOs.SaleOrder
{
    public class SaleOrderDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public BasicInfoCustomerDto Customer { get; set; } = null!;
        public int? CantItems { get; set; }
        public decimal TotalToPay { get; set; }
        public int OpportunityId { get; set; }
        public OpportunityDto Opportunity { get; set; } = null!;
        public int? FinancingPlanId { get; set; }
        public FinancingPlanDto FinancingPlan { get; set; } = null!;
        public int StatusId { get; set; }
        public StatusDto Status { get; set; } = null!;
        public bool IsActive { get; set; }
        public List<SaleOrderProductDto>? SaleOrderProducts { get; set; }
    }
}
