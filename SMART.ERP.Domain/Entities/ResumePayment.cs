namespace SMART.ERP.Domain.Entities
{
    public class ResumePayment
    {
        public int Id { get; init; }
        public int DailyCloseId { get; set; }
        public DailyClose? DailyClose { get; set; }
        public int TypeOfPaymentMethodId { get; set; }
        public TypeOfPaymentMethod? TypeOfPayment { get; set; }
        public decimal Amount { get; set; }
    }
}
