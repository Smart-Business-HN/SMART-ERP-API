using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.AdvisorGoal
{
    public class AdvisorGoalDto
    {
        public int Id { get; init; }
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
        public decimal Goal { get; set; }
        public DateTime InitDate { get; set; }
        public bool Enabled { get; set; }
        public DateTime EndDate { get; set; }
    }
}
