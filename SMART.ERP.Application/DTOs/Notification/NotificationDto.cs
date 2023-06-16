using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.Notification
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public string? Icon { get; set; }
        public string? Image { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Time { get; set; }
        public string? Link { get; set; }
        public bool UseRouter { get; set; }
        public bool Read { get; set; }
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
    }
}
