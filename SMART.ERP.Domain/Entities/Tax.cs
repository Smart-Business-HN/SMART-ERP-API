namespace SMART.ERP.Domain.Entities
{
    public class Tax
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public decimal Rate { get; set; }
        public string TextForDocuments { get; set; } = null!;
    }
}
