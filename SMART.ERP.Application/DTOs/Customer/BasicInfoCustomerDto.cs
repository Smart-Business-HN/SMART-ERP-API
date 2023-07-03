namespace SMART.ERP.Application.DTOs.Customer
{
    public class BasicInfoCustomerDto
    {
        public Guid MotorsId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public Guid? AssignedUserId { get; set; }
    }
}
