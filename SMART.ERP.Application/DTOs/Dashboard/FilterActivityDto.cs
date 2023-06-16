namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class FilterActivityDto
    {
        public Guid? UserId { get; set; }
        public int? BranchOfficeId { get; set; }
        public DateTime InitDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
