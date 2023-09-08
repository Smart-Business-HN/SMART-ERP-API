namespace SMART.ERP.Domain.Entities
{
    public class DeliveryDirection
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string AdditionalInformation { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public int CityId { get; set; }
        public virtual City? City { get; set; }
        public bool IsActive { get; set; }
        public bool IsFavorite { get; set; }
        public Guid CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
    }
}
