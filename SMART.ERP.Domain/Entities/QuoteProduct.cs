namespace SMART.ERP.Domain.Entities
{
    public class QuoteProduct
    {
        public int Id { get; init; }
        public int OpportunityId { get; set; }
        public virtual Opportunity? Opportunity { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsActive { get; set; }
    }
}
