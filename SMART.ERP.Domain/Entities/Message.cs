using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class Message
    {
        public int Id { get; init; }
        [MaxLength(30)]
        public string FirstName { get; set; } = null!;
        [MaxLength(30)]
        public string LastName { get; set; } = null!;
        [Column(TypeName = "varchar(50)")]
        public string FullName { get; set; } = null!;
        [Column(TypeName = "varchar(50)")]
        public string Subject { get; set; } = null!;
        [Column(TypeName = "varchar(30)")]
        public string Email { get; set; } = null!;
        [Column(TypeName = "varchar(15)")]
        public string PhoneNumber { get; set; } = null!;
        [Column(TypeName = "varchar(max)")]
        public string MessageContent { get; set; } = null!;
        public int CountryId { get; set; }
        public virtual Country? Country { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Date { get; set; }
        public Guid? CustomerId { get; set; }
        public bool IsRead { get; set; }
    }
}
