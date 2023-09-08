namespace SMART.ERP.Application.DTOs.Company
{
    public class TaxDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Rate { get; set; }
        public string TextForDocuments { get; set; } = null!;
    }
}
