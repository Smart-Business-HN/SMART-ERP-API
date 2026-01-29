using Microsoft.EntityFrameworkCore;

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
        [Precision(18, 2)]
        public decimal SalePrice { get; set; }
        public bool IsActive { get; set; }
    }
}
