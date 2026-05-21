namespace SMART.ERP.Application.DTOs.Customer
{
    public class CustomerTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
        public int? PriceListId { get; set; }
        public string? PriceListName { get; set; }
    }
}
