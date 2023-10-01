using SMART.ERP.Application.DTOs.Address;

namespace SMART.ERP.Application.DTOs.Customer
{
    public class DeliveryDirectionDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string AdditionalInformation { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public int CityId { get; set; }
        public CityDto? City { get; set; }
        public bool IsActive { get; set; }
        public bool IsFavorite { get; set; }
        public Guid CustomerId { get; set; }
    }
}
