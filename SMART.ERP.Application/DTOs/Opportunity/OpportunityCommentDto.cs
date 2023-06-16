using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.Opportunity
{
    public class OpportunityCommentDto
    {
        public string Message { get; set; } = null!;
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
        public int OpportunityId { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
