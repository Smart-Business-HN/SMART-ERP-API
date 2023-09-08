using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.DTOs.Quotation
{
    public class KindOfTaxDto
    {
        public Tax Tax { get; set; } = null!;
        public decimal Amount { get; set; }

    }
}
