namespace SMART.ERP.Application.DTOs.Customer
{
    public class CustomerTypeDto
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
