namespace SMART.ERP.Application.DTOs.Dashboard
{
    public class ProductComparativeSaleDto
    {
        public string Product { get; set; } = null!;
        public int LastYear { get; set; }
        public int CurrentYear { get; set; }
    }
}
