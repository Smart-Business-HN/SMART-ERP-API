using SMART.ERP.Application.DTOs.DailyClose;
using SMART.ERP.Application.DTOs.TypeOfPaymentMethod;

namespace SMART.ERP.Application.DTOs.ResumePayment
{
    public class ResumePaymentDto
    {
        public int Id { get; set; }
        public int DailyCloseId { get; set; }
        public DailyCloseDto? DailyClose { get; set; }
        public int TypeOfPaymentMethodId { get; set; }
        public TypeOfPaymentMethodDto? TypeOfPayment { get; set; }
        public decimal Amount { get; set; }
    }
}
