namespace SMART.ERP.Domain.Entities
{
    public class User
    {
        public Guid Id { get; init; }
        public string UserName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Photo { get; set; }
        public string Email { get; set; } = null!;
        public bool ConfirmedEmail { get; set; }
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
        public decimal? SalesGoal { get; set; }
        public virtual BranchOffices? BranchOffice { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public List<AdvisorDepartment>? Departments { get; set; }
    }
}
