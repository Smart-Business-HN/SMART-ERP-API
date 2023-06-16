namespace SMART.ERP.Domain.Entities
{
    public class CustomerMachinery
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public int BaseInfoId { get; set; }
        public DateTime? NextMaintenance { get; set; }
    }
}

