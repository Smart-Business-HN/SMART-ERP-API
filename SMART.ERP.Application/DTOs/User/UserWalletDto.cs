using SMART.ERP.Application.DTOs.Customer;
using SMART.ERP.Application.DTOs.Dashboard;

namespace SMART.ERP.Application.DTOs.User
{
    public class UserWalletDto
    {
        public UserDto? User { get; set; }
        public int? NumClient { get; set; }
        public int? NumOpportunity { get; set; }
        public int? NumTemporalClients { get; set; }
        public AdvisorDashboardDto? AdvisorInfo { get; set; }
        public List<ClientWalletDto>? Clients { get; set; }
    }
}
