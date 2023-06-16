using SMART.ERP.Application.DTOs.Status;
using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.Opportunity
{
    public class OpportunityActivityDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;
        public DateTime InitDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ExpiratedDays
        {
            get
            {
                return (EndDate - DateTime.Now).Days;
            }
        }
        public int TypeActivityId { get; set; }
        public TypeActivityDto? TypeActivity { get; set; }
        public int StatusId { get; set; }
        public StatusDto? Status { get; set; }
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
        public int OpportunityId { get; set; }
    }
}
