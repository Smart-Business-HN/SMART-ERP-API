namespace SMART.MASTER.Domain.Entities
{
    public class ClientCurrency
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
