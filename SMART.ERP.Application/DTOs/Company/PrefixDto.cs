using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.DTOs.Company
{
    public class PrefixDto
    {
        public int Id { get; set; }
        public string Format { get; set; } = null!;
        public int InternalDocumentId { get; set; }
        public InternalDocument? InternalDocument { get; set; }
        public bool ItIsTaken { get; set; }
    }
}
