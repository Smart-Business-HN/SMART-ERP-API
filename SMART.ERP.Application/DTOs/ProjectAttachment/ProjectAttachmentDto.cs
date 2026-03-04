using SMART.ERP.Application.DTOs.ProjectAttachmentCategory;
using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.ProjectAttachment
{
    public class ProjectAttachmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
        public int ProjectAttachmentCategoryId { get; set; }
        public ProjectAttachmentCategoryDto? ProjectAttachmentCategory { get; set; }
        public int ProjectId { get; set; }
    }
}
