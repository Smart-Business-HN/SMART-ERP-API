namespace SMART.ERP.Domain.Entities
{
    public class Prefix
    {
        public int Id { get; init; }
        public string Format { get; set; } = null!;
        public int InternalDocumentId { get; set; }
        public virtual InternalDocument InternalDocument { get; set; } = null!;
        public bool ItIsTaken { get; set; }
       
    }
}
