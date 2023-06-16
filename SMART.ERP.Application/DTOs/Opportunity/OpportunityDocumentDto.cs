using SMART.ERP.Application.DTOs.User;

namespace SMART.ERP.Application.DTOs.Opportunity
{
    public class OpportunityDocumentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Url { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public Guid UserId { get; set; }
        public UserDto? User { get; set; }
        public int DocumentTypeId { get; set; }
        public DocumentTypeDto? DocumentType { get; set; }
        public int OpportunityId { get; set; }
    }
}
