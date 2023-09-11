using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.DTOs.Company
{
    public class PrefixDto
    {
        public int Id { get; init; }
        public string Format { get; set; } = null!;
        public int InternalDocumentId { get; set; }
        public virtual InternalDocumentDto InternalDocument { get; set; } = null!;
        public bool ItIsTaken { get; set; }
    }
}
