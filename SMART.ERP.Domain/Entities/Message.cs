namespace SMART.ERP.Domain.Entities
{
    public class Message
    {
        public int Id { get; init; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string MessageContent { get; set; } = null!;
        public int CountryId { get; set; }
        public virtual Country? Country { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public DateTime Date { get; set; }
        public Guid? CustomerId { get; set; }
        public bool IsRead { get; set; }
    }
}
