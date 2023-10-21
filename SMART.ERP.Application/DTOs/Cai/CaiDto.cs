namespace SMART.ERP.Application.DTOs.Cai
{
    public class CaiDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? BranchOfficeId { get; set; }
        public string Prefix { get; set; } = null!;
        public string Identificator { get; set; } = null!;
        public int StartCorrelative { get; set; }
        public int EndCorrelative { get; set; }
        public int CurrentCorrelative { get; set; }
        public int AvailableInvoices { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidUntil { get; set; }
        public bool IsGeneralCai { get; set; }
        public bool IsActive { get; set; }
    }
}
