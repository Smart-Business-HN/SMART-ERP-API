namespace SMART.ERP.Domain.Entities
{
    public class Discount
    {
        public int Id { get; init; }
        public string Name { get; set; } = null!;
        public decimal Rate { get; set; }
    }
}