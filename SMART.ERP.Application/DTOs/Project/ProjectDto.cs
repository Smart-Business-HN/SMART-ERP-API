using SMART.ERP.Application.DTOs.Company;
using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.DTOs.Status;

namespace SMART.ERP.Application.DTOs.Project
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ProjectCode { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal ExecutionBudget { get; set; }
        public Guid CustomerId { get; set; }
        public CustomerDto? Customer { get; set; }
        public int StatusId { get; set; }
        public StatusDto? Status { get; set; }
        public int PrefixId { get; set; }
        public PrefixDto? Prefix { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime InsertedDate { get; set; }
        public string? ModificatedBy { get; set; }
        public DateTime? ModificationDate { get; set; }
    }
}
