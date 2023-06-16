namespace SMART.ERP.Domain.Entities
{
    public class MetaConversations
    {
        public string Phone { get; set; } = null!;
        public int Expiration { get; set; }
        public bool ExpectingInfo { get; set; }
        public int ProductId { get; set; }
    }
}
