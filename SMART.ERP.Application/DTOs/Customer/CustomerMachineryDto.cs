using SMART.ERP.Application.DTOs.Product;

namespace SMART.ERP.Application.DTOs.Customer
{
    public class CustomerMachineryDto
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerDto Customer { get; set; } = null!;
        public int ProductId { get; set; }
        public ProductDto Product { get; set; } = null!;
        public int BaseInfoId { get; set; }
        public DateOnly? NextMaintenance { get; set; }
    }
}

