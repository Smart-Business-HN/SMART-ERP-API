using System.ComponentModel.DataAnnotations;

namespace SMART.ERP.Domain.Entities
{
    public class DeliveryDirection
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string Description { get; set; } = null!;
        [MaxLength(50)]
        public string PhoneNumber { get; set; } = null!;
        [MaxLength(500)]
        public string AdditionalInformation { get; set; } = null!;
        [MaxLength(50)]
        public string PostalCode { get; set; } = null!;
        public int CityId { get; set; }
        public virtual City? City { get; set; }
        public bool IsActive { get; set; }
        public bool IsFavorite { get; set; }
        public Guid CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }
    }
}
