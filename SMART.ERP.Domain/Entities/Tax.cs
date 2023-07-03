namespace SMART.ERP.Domain.Entities
{
    public class Tax
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public int Rate { get; set; }
        public string TextForDocuments { get; set; } = null!;
    }
}
