namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class ProductComparativeDto
    {
        public string Product { get; set; } = null!;
        public int SoldPast { get; set; }
        public int SoldCurrent { get; set; }
        public decimal SoldPastTotal { get; set; }
        public decimal SoldCurrentTotal { get; set; }
    }
}
