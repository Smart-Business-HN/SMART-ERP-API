namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class OpportunityAdvisorDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int NumOpportunities { get; set; }
        public string BranchOffice { get; set; } = null!;
    }
}
