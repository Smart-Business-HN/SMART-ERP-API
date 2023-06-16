namespace SMART.ERP.Domain.Entities
{
    public class SaleOrder
    {
        public int Id { get; init; }
        public string Code { get; set; } = null!;
        public Guid CustomerId { get; set; }
        public virtual Customer? Customer { get; set; } 
        public int? CantItems { get; set; }
        public decimal TotalToPay { get; set; }
        public int OpportunityId { get; set; }
        public virtual Opportunity? Opportunity { get; set; }
        public int? FinancingPlanId { get; set; }
        public virtual FinancingPlan? FinancingPlan { get; set; }
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
        public bool IsActive { get; set; }
        public List<SaleOrderProduct>? SaleOrderProducts { get; set; }
    }
}
