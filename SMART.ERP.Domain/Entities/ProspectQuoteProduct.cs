namespace SMART.ERP.Domain.Entities
{
    public class ProspectQuoteProduct
    {
        public int Id { get; init; }
        public Guid ProspectId { get; set; }
        public virtual Prospect? Prospect { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public bool IsActive { get; set; }
    }
}
