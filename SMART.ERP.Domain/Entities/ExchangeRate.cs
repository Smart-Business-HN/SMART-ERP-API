namespace SMART.ERP.Domain.Entities
{
    public class ExchangeRate
    {
        public int Id { get; init; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
        public int CurrencyId { get; set; }
        public virtual Currency? Currency { get; set; }
    }
}
