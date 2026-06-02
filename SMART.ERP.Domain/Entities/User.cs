using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMART.ERP.Domain.Entities
{
    public class User
    {
        public Guid Id { get; init; }
        [Column(TypeName = "varchar(30)")]
        public string UserName { get; set; } = null!;
        [Column(TypeName = "varchar(max)")]
        public string FullName { get; set; } = null!;
        [Column(TypeName = "varchar(30)")]
        public string FirstName { get; set; } = null!;
        [Column(TypeName = "varchar(30)")]
        public string LastName { get; set; } = null!;
        [Column(TypeName = "varchar(max)")]
        public string? Photo { get; set; }
        [Column(TypeName = "varchar(30)")]
        public string Email { get; set; } = null!;
        public bool ConfirmedEmail { get; set; }
        [Column(TypeName = "varchar(10)")]
        public string PhoneNumber { get; set; } = null!;
        public bool ConfirmedPhoneNumber { get; set; }
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public byte[] MasterPasswordHash { get; set; } = null!;
        public byte[] MasterPasswordSalt { get; set; } = null!;
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public virtual Role? Role { get; set; }
        public int GenderId { get; set; }
        public virtual Gender? Gender { get; set; }
        public int? BranchOfficeId { get; set; }
        [Precision(18, 2)]
        public decimal? SalesGoal { get; set; }
        [Precision(5, 2)]
        public decimal? CommissionPercentage { get; set; }
        public virtual BranchOffices? BranchOffice { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public List<AdvisorDepartment>? Departments { get; set; }
    }
}
